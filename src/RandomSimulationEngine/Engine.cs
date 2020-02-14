using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Rest;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SimpleInjector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using RandomSimulationEngine.Rest.Throttling.Middlewares;

namespace RandomSimulationEngine
{
    /// <summary>
    /// Main engine of the app. It contains all the bad, non-injectable stuff.
    /// </summary>
    public static class Engine
    {
#warning TODO - jakoś to ładniej opakować, żeby się ładni enajpierw w tle workery odpalały a potem się hostowało, no i graceful shutdown ma być

        private static readonly ILog log = LogManager.GetLogger(typeof(Engine));

        private static IWebHost webHost;

        /// <summary>
        /// Gets main <see cref="SimpleInjector"/> container.
        /// </summary>
        public static Container InjectionContainer { get; } = new Container();

        /// <summary>
        /// Initialize <see cref="SimpleInjector"/>'s container. Should be executed at the beginning of the application.
        /// </summary>
        public static void InitializeContainer()
        {
            InjectionContainer.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;

            InjectionContainer.RegisterSingleton<IConfigurationReader, ConfigurationReader>();
            //InjectionContainer.RegisterSingleton<IRandomNamePuller, RandomNamePuller>();
            //InjectionContainer.RegisterSingleton<IHtmlBuilder, HtmlBuilder>();

            InjectionContainer.Register<IRandomSimulationController, RandomSimulationController>(Lifestyle.Scoped);

            log.Debug("Container verification attempt");
            InjectionContainer.Verify();
        }

        /// <summary>
        /// Start REST service hosting.
        /// </summary>
        public static void StartHosting()
        {
            ThrottlingConfiguration throttling = InjectionContainer.GetInstance<ConfigurationReader>().ReadConfiguration().Throttling;

            webHost = WebHost.CreateDefaultBuilder(null)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Limits.MaxConcurrentConnections = throttling.MaximumServerConnections;
                })
                .ConfigureServices(services =>
                {
                    log.Debug("Startup.ConfigureServices");

                    services.AddSingleton<IControllerFactory, ControllerFactory>();
                    services.AddLogging();
                    services.AddMvc();
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ThrottlingMiddleware>(throttling);
                    app.UseMvc();
                })
                .Build();
            webHost.Run();
        }

        /// <summary>
        /// Stops REST service hosting.
        /// </summary>
        public static void StopHosting()
        {
            log.Info("Hosting stoppingt");
            webHost.StopAsync().Wait();
        }
    }
}
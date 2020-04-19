using log4net;
using Microsoft.AspNetCore.Mvc.Controllers;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.DateTime;
using RandomSimulationEngine.Factories.ImageDownload;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Rest;
using RandomSimulationEngine.Tasks;
using RandomSimulationEngine.ValueCalculator;
using SimpleInjector;

namespace RandomSimulationEngine
{
    public static class ContainerRegistrator
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ContainerRegistrator));

        public static Container Register()
        {
            _log.Info("Registering container");

            Container container = new Container();

            container.Options.DefaultScopedLifestyle = ScopedLifestyle.Flowing;

            container.RegisterSingleton<IDateTimeService, DateTimeService>();
            container.RegisterSingleton<IRandomService, RandomService>();

            container.RegisterSingleton<IConfigurationReader, ConfigurationReader>();

            container.RegisterSingleton<IImageDownloadTaskFactory, ImageDownloadTaskFactory>();
            container.RegisterSingleton<ITaskMaster, TaskMaster>();

            container.RegisterSingleton<IValueCalculator, ValueCalculator.ValueCalculator>();
            container.RegisterSingleton<IRandomBytesPuller, RandomBytesPuller.RandomBytesPuller>();
            container.RegisterSingleton<IHealthChecker, HealthChecker>();

            container.Register<IRandomSimulationController, RandomSimulationController>(Lifestyle.Scoped);
            container.RegisterSingleton<IControllerFactory, ControllerFactory>();

            container.RegisterSingleton<IEngine, Engine>();

            _log.Debug("Container verification attempt");
            container.Verify();

            return container;
        }
    }
}
using System.Linq;
using System.Threading;
using RandomSimulationEngine.Configuration;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RandomSimulationEngine.Factories.ImageDownload;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Rest.Throttling.Middlewares;
using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine
{
    /// <summary>
    /// Main engine of the app. It contains all the bad, non-injectable stuff.
    /// </summary>
    public class Engine : IEngine
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Engine));

        private IWebHost _webHost;

        private readonly IConfigurationReader _configurationReader;
        private readonly IControllerFactory _controllerFactory;
        private readonly IImageDownloadTaskFactory _imageDownloadTaskFactory;
        private readonly ITaskMaster _taskMaster;
        private readonly IRandomBytesPuller _randomBytesPuller;
        private readonly IHealthChecker _healthChecker;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public Engine
        (
            IConfigurationReader configurationReader,
            IControllerFactory controllerFactory,
            IImageDownloadTaskFactory imageDownloadTaskFactory,
            ITaskMaster taskMaster,
            IRandomBytesPuller randomBytesPuller,
            IHealthChecker healthChecker
        )
        {
            _configurationReader = configurationReader;
            _controllerFactory = controllerFactory;
            _imageDownloadTaskFactory = imageDownloadTaskFactory;
            _taskMaster = taskMaster;
            _randomBytesPuller = randomBytesPuller;
            _healthChecker = healthChecker;
        }

        public void Start()
        {
            _log.Info("Starting data acquisition");
            StartDataAcquisition();

            _log.Info("Starting engine hosting");
            StartHosting();
        }

        public void Stop()
        {
            _log.Info("Stopping data acquisition");
            StopDataAcquisition();

            _log.Info("Stopping engine hosting");
            StopHosting();
        }

        private void StartDataAcquisition()
        {
            _log.Info($"Creating tasks of count {_configurationReader.Configuration.ImageDownload.FrameGrabUrls.Count}");
#warning DEBUG !!!!!
            foreach (string url in _configurationReader.Configuration.ImageDownload.FrameGrabUrls.Take(1))
                //foreach (string url in _configurationReader.Configuration.ImageDownload.FrameGrabUrls)
            {
                ISourceTask sourceTask = _imageDownloadTaskFactory.GetNewTask(url);

                _taskMaster.Register(sourceTask);
                _randomBytesPuller.Register(sourceTask);
                _healthChecker.Register(sourceTask);
            }

            _taskMaster.StartTasks(_tokenSource.Token);
        }

        /// <summary>
        /// Start REST service hosting.
        /// </summary>
        protected virtual void StartHosting()
        {
            ThrottlingConfiguration throttling = _configurationReader.Configuration.Throttling;

            _webHost = WebHost.CreateDefaultBuilder(null)
                .UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Limits.MaxConcurrentConnections = throttling.MaximumServerConnections;
                })
                .ConfigureServices(services =>
                {
                    _log.Debug("Startup.ConfigureServices");

                    services.AddSingleton(s => _controllerFactory);
                    services.AddLogging();
                    services.AddMvc();

                    services.AddSwaggerGen(c => { c.SwaggerDoc("random-simulation", new OpenApiInfo {Title = "Random Simulation", Version = "v1"}); });
                })
                .Configure(app =>
                {
                    app.UseMiddleware<ThrottlingMiddleware>(throttling);
                    app.UseMvc();
                    app.UseSwagger();

                    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/random-simulation/swagger.json", "Random Simulation"); });
                })
                .Build();

            _webHost.Run();
        }

        /// <summary>
        /// Stops REST service hosting.
        /// </summary>
        protected virtual void StopHosting()
        {
            _log.Info("Hosting stopping");
            _webHost.StopAsync().Wait();
            _log.Info("Hosting stopped");
        }

        private void StopDataAcquisition()
        {
            _tokenSource.Cancel();
        }
    }
}
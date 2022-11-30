using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    public sealed class TestServer : IDisposable
    {
        private readonly IWebHost _webHost;

        public TestServer()
        {
            _webHost = StartHosting();
        }

        /// <summary>
        /// Start REST service hosting.
        /// </summary>
        private IWebHost StartHosting()
        {
            IWebHost webHost = WebHost.CreateDefaultBuilder()
                .UseKestrel(options => { options.AddServerHeader = false; })
                .ConfigureServices(services => { services.AddMvc(mvcOptions => { mvcOptions.EnableEndpointRouting = false; }); })
                .Configure(app => { app.UseMvc(); })
                .Build();
            webHost.StartAsync();
            return webHost;
        }

        /// <summary>
        /// Stops REST service hosting.
        /// </summary>
        private void StopHosting()
        {
            _webHost.StopAsync().Wait();
        }

        public void Dispose()
        {
            StopHosting();
            _webHost.Dispose();
        }
    }
}
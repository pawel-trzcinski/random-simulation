using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    public sealed class TestServer : IDisposable
    {
        private IWebHost _webHost;

        public TestServer()
        {
            StartHosting();
        }

        /// <summary>
        /// Start REST service hosting.
        /// </summary>
        private void StartHosting()
        {
            _webHost = WebHost.CreateDefaultBuilder(null)
                .UseKestrel(options => { options.AddServerHeader = false; })
                .ConfigureServices(services => { services.AddMvc(); })
                .Configure(app => { app.UseMvc(); })
                .Build();
            _webHost.StartAsync();
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
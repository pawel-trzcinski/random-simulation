using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    [TestFixture]
    public class WebClientWrapperTests
    {
        [Test]
        public async Task GetsTestGuid()
        {
            using (new TestServer())
            {
                WebClientWrapper wrapper = new WebClientWrapper();
                byte[] bytes = await wrapper.GetImageBytes($"http://127.0.0.1:15500/{TestController.ACTION_VERB}", CancellationToken.None);
                string guidString = Encoding.UTF8.GetString(bytes);

                Assert.That(Guid.TryParse(guidString, out _));
            }
        }
    }
}

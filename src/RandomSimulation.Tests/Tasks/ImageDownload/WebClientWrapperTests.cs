using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    [TestFixture]
    public class WebClientWrapperTests
    {
        [Test]
        public void GetsTestGuid()
        {
            using (new TestServer())
            {
                WebClientWrapper wrapper = new WebClientWrapper();
                byte[] bytes = wrapper.GetImageBytes($"http://127.0.0.1:15500/{TestController.ACTION_VERB}", CancellationToken.None);
                string guidString = Encoding.UTF8.GetString(bytes);

                Assert.True(Guid.TryParse(guidString, out _));
            }
        }
    }
}
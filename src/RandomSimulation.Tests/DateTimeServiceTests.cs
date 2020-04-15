using System;
using NUnit.Framework;
using RandomSimulationEngine.DateTime;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class DateTimeServiceTests
    {
        [Test]
        public void ServiceReturnsDate()
        {
            DateTimeService service = new DateTimeService();
            Assert.AreNotEqual(DateTime.MaxValue, service.UtcNow);
            Assert.AreNotEqual(DateTime.MinValue, service.UtcNow);

            DateTime serviceUtcNow = service.UtcNow;

            Assert.IsTrue(DateTime.UtcNow - serviceUtcNow < TimeSpan.FromSeconds(1));
        }
    }
}
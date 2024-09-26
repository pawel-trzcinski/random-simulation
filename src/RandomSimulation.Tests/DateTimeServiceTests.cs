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
            Assert.That(DateTime.MaxValue, Is.Not.EqualTo(service.UtcNow));
            Assert.That(DateTime.MinValue, Is.Not.EqualTo(service.UtcNow));

            DateTime serviceUtcNow = service.UtcNow;

            Assert.That(DateTime.UtcNow - serviceUtcNow < TimeSpan.FromSeconds(1));
        }
    }
}

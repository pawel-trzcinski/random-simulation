using NUnit.Framework;
using RandomSimulationEngine;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class ContainerRegistratorTests
    {
        [Test]
        public void ContainerRegistersCorrectly()
        {
            Assert.That(ContainerRegistrator.Register(), Is.Not.Null);
        }
    }
}
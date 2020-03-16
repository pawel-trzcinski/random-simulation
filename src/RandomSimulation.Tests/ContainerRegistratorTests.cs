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
            Assert.DoesNotThrow(() => { Assert.IsNotNull(ContainerRegistrator.Register()); });
        }
    }
}
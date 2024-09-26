using System;
using System.Linq;
using NUnit.Framework;
using RandomSimulationEngine.Tasks;

namespace RandomSimulation.Tests.Tasks;

[TestFixture]
public class BytesProvidingResultTests
{
    [Test]
    public void EmptyResultsHasNoData()
    {
        BytesProvidingResult emptyResult = BytesProvidingResult.Empty();
        Assert.That(emptyResult.IsDtataAvailable, Is.False);
        Assert.That(emptyResult.Data, Is.Empty);
    }

    [Test]
    public void EmptyInputArrayProducesEmptyResult()
    {
        BytesProvidingResult emptyResult = BytesProvidingResult.Create(Array.Empty<byte>());
        Assert.That(emptyResult.IsDtataAvailable, Is.False);
        Assert.That(emptyResult.Data, Is.Empty);
    }

    [Test]
    public void ResultWithDataReturnsCorrectValues([Values(1, 2, 3, 5, 10, 100, 10000)]int arrayLength)
    {
        byte[] bytes = new byte[arrayLength];
        new Random().NextBytes(bytes);

        BytesProvidingResult result = BytesProvidingResult.Create(bytes);
        Assert.That(result.IsDtataAvailable);
        Assert.That(result.Data, Is.Not.Empty);
        Assert.That(bytes.SequenceEqual(result.Data));
    }
}
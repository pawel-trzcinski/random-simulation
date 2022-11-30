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
        Assert.IsFalse(emptyResult.IsDtataAvailable);
        Assert.IsEmpty(emptyResult.Data);
    }

    [Test]
    public void EmptyInputArrayProducesEmptyResult()
    {
        BytesProvidingResult emptyResult = BytesProvidingResult.Create(Array.Empty<byte>());
        Assert.IsFalse(emptyResult.IsDtataAvailable);
        Assert.IsEmpty(emptyResult.Data);
    }

    [Test]
    public void ResultWithDataReturnsCorrectValues([Values(1, 2, 3, 5, 10, 100, 10000)]int arrayLength)
    {
        byte[] bytes = new byte[arrayLength];
        new Random().NextBytes(bytes);

        BytesProvidingResult result = BytesProvidingResult.Create(bytes);
        Assert.IsTrue(result.IsDtataAvailable);
        Assert.IsNotEmpty(result.Data);
        Assert.IsTrue(bytes.SequenceEqual(result.Data));
    }
}
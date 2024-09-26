using MathNet.Numerics.Statistics;
using NUnit.Framework;
using RandomSimulationEngine.History.Report;

namespace RandomSimulation.Tests.History
{
    [TestFixture]
    public class OperationReportTests
    {
        [Test]
        public void CsvHasCorrectValue()
        {
            double[] testArray = new double[100];
            for (int i = 0; i < 100; i++)
            {
                testArray[i] = i * 3 + i * i;
            }

            Histogram histogram = new Histogram(testArray,10);

            OperationReport report = new OperationReport(histogram);
            Assert.That(report.Csv, Is.EqualTo("31;13;10;9;7;7;6;6;5;6"));
        }
    }
}
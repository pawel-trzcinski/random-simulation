using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Statistics;
using NUnit.Framework;
using RandomSimulationEngine.History.Report;

namespace RandomSimulation.Tests.History
{
    [TestFixture]
    public class HistogramReportTests
    {
        private const double EPSILON = 0.001;

        [Test]
        public void HistogramsGoToTheirSpecificProperties()
        {
            IEnumerable<double> testArrayNext = Enumerable.Range(0, 50).Select(Convert.ToDouble);
            IEnumerable<double> testArrayNextMax = Enumerable.Range(100, 50).Select(Convert.ToDouble);
            IEnumerable<double> testArrayNextMinMax = Enumerable.Range(200, 50).Select(Convert.ToDouble);
            IEnumerable<double> testArrayNextDouble = Enumerable.Range(300, 50).Select(Convert.ToDouble);
            IEnumerable<double> testArrayNextBytes = Enumerable.Range(400, 50).Select(Convert.ToDouble);

            HistogramReport report = new HistogramReport
            (
                new Histogram(testArrayNext, 20),
                new Histogram(testArrayNextMax, 20),
                new Histogram(testArrayNextMinMax, 20),
                new Histogram(testArrayNextDouble, 20),
                new Histogram(testArrayNextBytes, 20)
            );

            Assert.That(Math.Abs(0 - report.Next.Histogram.LowerBound) < EPSILON);
            Assert.That(Math.Abs(100 - report.NextMax.Histogram.LowerBound) < EPSILON);
            Assert.That(Math.Abs(200 - report.NextMinMax.Histogram.LowerBound) < EPSILON);
            Assert.That(Math.Abs(300 - report.NextDouble.Histogram.LowerBound) < EPSILON);
            Assert.That(Math.Abs(400 - report.NextBytes.Histogram.LowerBound) < EPSILON);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MathNet.Numerics.Statistics;

namespace RandomSimulationEngine.History.Report
{
    public class OperationReport
    {
        public Histogram Histogram { get; }

        public string Csv { get; }

        public OperationReport(Histogram histogram)
        {
            Histogram = histogram;

            List<double> list = new List<double>(histogram.BucketCount);
            for (int i = 0; i < histogram.BucketCount; i++)
            {
                list.Add(histogram[i].Count);
            }

            this.Csv = String.Join(";", list.Select(p => p.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
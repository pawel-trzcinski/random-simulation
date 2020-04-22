using JetBrains.Annotations;
using MathNet.Numerics.Statistics;

namespace RandomSimulationEngine.History.Report
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class HistogramReport
    {
        public OperationReport Next { get; }
        public OperationReport NextMax { get; }
        public OperationReport NextMinMax { get; }
        public OperationReport NextDouble { get; }
        public OperationReport NextBytes { get; }

        public HistogramReport
        (
            Histogram histogramNext,
            Histogram histogramNextMax,
            Histogram histogramNextMinMax,
            Histogram histogramNextDouble,
            Histogram histogramNextBytes
        )
        {
            Next = new OperationReport(histogramNext);
            NextMax = new OperationReport(histogramNextMax);
            NextMinMax = new OperationReport(histogramNextMinMax);
            NextDouble = new OperationReport(histogramNextDouble);
            NextBytes = new OperationReport(histogramNextBytes);
        }
    }
}
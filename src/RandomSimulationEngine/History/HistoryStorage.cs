using System.Collections.Generic;
using System.Linq;
using log4net;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.History.Report;

namespace RandomSimulationEngine.History
{
    public class HistoryStorage : IHistoryStorage
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(HistoryStorage));

        private readonly HistoryConfiguration _configuration;
        
        private readonly object _lockNext = new object();
        protected readonly LinkedList<double> _listNext = new LinkedList<double>();

        private readonly object _lockNextMax = new object();
        protected readonly LinkedList<double> _listNextMax = new LinkedList<double>();

        private readonly object _lockNextMinMax = new object();
        protected readonly LinkedList<double> _listNextMinMax = new LinkedList<double>();

        private readonly object _lockNextDouble = new object();
        protected readonly LinkedList<double> _listNextDouble = new LinkedList<double>();

        private readonly object _lockNextBytes = new object();
        protected readonly LinkedList<double> _listNextBytes = new LinkedList<double>();

        public HistoryStorage(IConfigurationReader configurationReader)
        {
            _configuration = configurationReader.Configuration.History;
        }

        public void StoreNext(int next)
        {
            lock (_lockNext)
            {
                _listNext.AddLast(next);

                if (_listNext.Count > _configuration.IntSamples)
                {
                    _listNext.RemoveFirst();
                }
            }
        }

        public void StoreNextMax(int next)
        {
            lock (_lockNextMax)
            {
                _listNextMax.AddLast(next);

                if (_listNextMax.Count > _configuration.IntSamples)
                {
                    _listNextMax.RemoveFirst();
                }
            }
        }

        public void StoreNextMinMax(int next)
        {
            lock (_lockNextMinMax)
            {
                _listNextMinMax.AddLast(next);

                if (_listNextMinMax.Count > _configuration.IntSamples)
                {
                    _listNextMinMax.RemoveFirst();
                }
            }
        }

        public void StoreNextDouble(double next)
        {
            lock (_lockNextDouble)
            {
                _listNextDouble.AddLast(next);

                if (_listNextDouble.Count > _configuration.DoubleSamples)
                {
                    _listNextDouble.RemoveFirst();
                }
            }
        }

        public void StoreNextBytes(byte[] bytes)
        {
            lock (_lockNextBytes)
            {
                foreach (byte b in bytes)
                {
                    _listNextBytes.AddLast(b);
                }

                if (_listNextBytes.Count > _configuration.ByteSamples)
                {
                    int exces = _listNextBytes.Count - _configuration.ByteSamples;

                    for (int i = 0; i < exces; i++)
                    {
                        _listNextBytes.RemoveFirst();
                    }
                }
            }
        }

        public string GetHistogramReport(int bucketsCount)
        {
            _log.Info($"Generating histogram with buckets count of {bucketsCount}");

            IEnumerable<double> valuesNext;
            lock (_lockNext)
            {
                valuesNext = _listNext.ToArray();
            }

            IEnumerable<double> valuesNextMax;
            lock (_lockNextMax)
            {
                valuesNextMax = _listNextMax.ToArray();
            }

            IEnumerable<double> valuesNextMinMax;
            lock (_lockNextMinMax)
            {
                valuesNextMinMax = _listNextMinMax.ToArray();
            }

            IEnumerable<double> valuesNextDouble;
            lock (_lockNextDouble)
            {
                valuesNextDouble = _listNextDouble.ToArray();
            }

            IEnumerable<double> valuesNextBytes;
            lock (_lockNextBytes)
            {
                valuesNextBytes = _listNextBytes.ToArray();
            }

            Histogram histogramNext = new Histogram(!valuesNext.Any() ? new double[]{0} : valuesNext, bucketsCount);
            Histogram histogramNextMax = new Histogram(!valuesNextMax.Any() ? new double[] { 0 } : valuesNextMax, bucketsCount);
            Histogram histogramNextMinMax = new Histogram(!valuesNextMinMax.Any() ? new double[] { 0 } : valuesNextMinMax, bucketsCount);
            Histogram histogramNextDouble = new Histogram(!valuesNextDouble.Any() ? new double[] { 0 } : valuesNextDouble, bucketsCount);
            Histogram histogramNextBytes = new Histogram(!valuesNextBytes.Any() ? new double[] { 0 } : valuesNextBytes, bucketsCount);

            return JsonConvert.SerializeObject(new HistogramReport
            (
                histogramNext,
                histogramNextMax,
                histogramNextMinMax,
                histogramNextDouble,
                histogramNextBytes
            ));
        }
    }
}
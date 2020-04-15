using System;
using JetBrains.Annotations;

namespace RandomSimulationEngine.ValueCalculator
{
    public class ValueCalculator : IValueCalculator
    {
        private static readonly double _originalMin = Convert.ToDouble(Int32.MinValue);
        private static readonly double _originalRange = Convert.ToDouble(Int32.MaxValue) - Convert.ToDouble(Int32.MinValue);

        public double GetDouble([NotNull] byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 8)
            {
                throw new ArgumentException($"{nameof(bytes)}.{nameof(bytes.Length)} should equal to 8", nameof(bytes));
            }

            return BitConverter.ToDouble(bytes, 0);
        }

        public int GetInt32([NotNull] byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException($"{nameof(bytes)}.{nameof(bytes.Length)} should equal to 4", nameof(bytes));
            }

            return BitConverter.ToInt32(bytes, 0);
        }

        public int GetInt32([NotNull] byte[] bytes, int max)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException($"{nameof(bytes)}.{nameof(bytes.Length)} should equal to 4", nameof(bytes));
            }

            if (max <= 0)
            {
                throw new ArgumentException($"{nameof(max)} must be a positive number", nameof(max));
            }

            return Normalize(BitConverter.ToInt32(bytes, 0), 0, max);
        }

        public int GetInt32([NotNull] byte[] bytes, int min, int max)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException($"{nameof(bytes)}.{nameof(bytes.Length)} should equal to 4", nameof(bytes));
            }

            return Normalize(BitConverter.ToInt32(bytes, 0), min, max);
        }

        private static int Normalize(int value, int min, int max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"{nameof(max)} has to be grater than {nameof(min)}", nameof(max));
            }

            // I'm lazy
            // https://stackoverflow.com/questions/5294955/how-to-scale-down-a-range-of-numbers-with-a-known-min-and-max-value
            //      (b - a)(x - min)
            //f(x) = --------------+a
            //       max - min

            double newMin = Convert.ToDouble(min);
            double newRange = Convert.ToDouble(max) - newMin;

            double originalInt = Convert.ToDouble(value);

            return Convert.ToInt32(newRange * (originalInt - _originalMin) / _originalRange + newMin);
        }
    }
}
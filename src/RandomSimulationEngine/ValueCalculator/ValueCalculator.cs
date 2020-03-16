using System;

namespace RandomSimulationEngine.ValueCalculator
{
    public class ValueCalculator : IValueCalculator
    {
#warning TODO - unit tests
        public double GetDouble(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 8)
            {
                throw new ArgumentException("bytes.Length should equal to 8", nameof(bytes));
            }

            return BitConverter.ToDouble(bytes, 0);
        }

        public int GetInt32(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes.Length should equal to 4", nameof(bytes));
            }

            return BitConverter.ToInt32(bytes, 0);
        }

        public int GetInt32(byte[] bytes, int max)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes.Length should equal to 4", nameof(bytes));
            }

            return Normalize(BitConverter.ToInt32(bytes, 0), 0, max);
        }

        public int GetInt32(byte[] bytes, int min, int max)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length != 4)
            {
                throw new ArgumentException("bytes.Length should equal to 4", nameof(bytes));
            }

            return Normalize(BitConverter.ToInt32(bytes, 0), min, max);
        }

        private static int Normalize(int value, int min, int max)
        {
            if (max <= min)
            {
                throw new ArgumentException($"{nameof(max)} has to be grater than {nameof(min)}", nameof(max));
            }

            double originalRange = Convert.ToDouble(Int32.MaxValue) - Convert.ToDouble(Int32.MinValue);
            double newRange = Convert.ToDouble(min) - Convert.ToDouble(max);

            double originalInt = Convert.ToDouble(value);

            // newInt/originalInt = newRange/originalRange
            // newInt = newRange * OriginalInt/OriginalRange

            return Convert.ToInt32(newRange * originalInt / originalRange);
        }
    }
}
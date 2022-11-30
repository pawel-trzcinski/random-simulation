using System;
using NUnit.Framework;
using RandomSimulationEngine.ValueCalculator;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class ValueCalculatorTests
    {
        private static readonly Random _random = new Random();

        private static readonly (int min, int max)[] _ranges =
        {
            (Int32.MinValue, 1),
            (-5, 1),
            (0, 1),
            (Int32.MinValue, 2),
            (-2, 2),
            (0, 2),
            (1, 2),
            (0, 10),
            (9, 10),
            (Int32.MinValue, Int32.MaxValue),
            (-100, Int32.MaxValue),
            (0, Int32.MaxValue),
            (1000, Int32.MaxValue),
            (Int32.MaxValue - 1, Int32.MaxValue)
        };

        #region GetDouble

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(7)]
        [TestCase(9)]
        [TestCase(16)]
        public void GetDouble_8BytesCheck(int dataLength)
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetDouble(inputArray));
        }

        [Test]
        [Repeat(100 * 1000)]
        public void GetDouble_Any8BytesArrayReturnsValue()
        {
            byte[] inputArray = Helper.GetRandomArray(8);
            ValueCalculator calculator = new ValueCalculator();
            Assert.DoesNotThrow(() => calculator.GetDouble(inputArray));
        }

        #endregion GetDouble

        #region GetInt32

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(8)]
        [TestCase(16)]
        public void GetInt32_4BytesCheck(int dataLength)
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetInt32(inputArray));
        }

        [Test]
        [Repeat(100 * 1000)]
        public void GetInt32_Any4BytesArrayReturnsValue()
        {
            byte[] inputArray = Helper.GetRandomArray(4);
            ValueCalculator calculator = new ValueCalculator();
            Assert.DoesNotThrow(() => calculator.GetInt32(inputArray));
        }

        #endregion GetInt32

        #region GetInt32Max

        [Test]
        [Combinatorial]
        public void GetInt32Max_4BytesCheck
        (
            [Values(0, 1, 2, 3, 5, 8, 16)] int dataLength,
            [Values(1, 2, 10, 100, Int32.MaxValue)]
            int max
        )
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetInt32(inputArray, max));
        }

        [Test]
        [Combinatorial]
        public void GetInt32Max_PositiveMaxCheck
        (
            [Values(0, 1, 2, 3, 5, 8, 16)] int dataLength,
            [Values(Int32.MinValue, -1000, -1, 0)] int max
        )
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetInt32(inputArray, max));
        }

        [Test]
        [Repeat(100 * 1000)]
        public void GetInt32Max_Any4BytesArrayReturnsValue()
        {
            int max = _random.Next(1, Int32.MaxValue);

            byte[] inputArray = Helper.GetRandomArray(4);
            ValueCalculator calculator = new ValueCalculator();
            int value = calculator.GetInt32(inputArray, max);
            Assert.IsTrue(value >= 0);
            Assert.IsTrue(value < max);
        }

        #endregion GetInt32Max

        #region GetInt32MinMax

        [Test]
        [Combinatorial]
        public void GetInt32MinMax_4BytesCheck
        (
            [Values(0, 1, 2, 3, 5, 8, 16)] int dataLength,
            [ValueSource(nameof(_ranges))] (int min, int max) range
        )
        {
            (int min, int max) = range;
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetInt32(inputArray, min, max));
        }

        [Test]
        [Combinatorial]
        public void GetInt32MinMax_MinLessThanMaxCheck
        (
            [Values(0, 1, 2, 3, 5, 8, 16)] int dataLength,
            [ValueSource(nameof(_ranges))] (int min, int max) range
        )
        {
            (int min, int max) = range;
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            ValueCalculator calculator = new ValueCalculator();
            Assert.Throws<ArgumentException>(() => calculator.GetInt32(inputArray, max, min));
        }

        [Test]
        [Repeat(100 * 1000)]
        public void GetInt32MinMax_Any4BytesArrayReturnsValue()
        {
            int min = _random.Next(1, Int32.MaxValue);
            int max = _random.Next(1, Int32.MaxValue);

            if (min > max)
            {
                (min, max) = (max, min);
            }
            else if (min == max)
            {
                ++max;
            }

            byte[] inputArray = Helper.GetRandomArray(4);
            ValueCalculator calculator = new ValueCalculator();
            int value = calculator.GetInt32(inputArray, min, max);
            Assert.IsTrue(value >= 0);
            Assert.IsTrue(value < max);
        }

        #endregion GetInt32MinMax
    }
}
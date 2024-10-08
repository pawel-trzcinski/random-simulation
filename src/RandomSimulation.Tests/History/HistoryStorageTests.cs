﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.History;

namespace RandomSimulation.Tests.History
{
    [TestFixture]
    public class HistoryStorageTests
    {
        private const int DEFAULT_SAMPLES_COUNT = HistoryConfiguration.MIN_SAMPLES;

        private static readonly Random _random = new ();

        private class HistoryStorageTester : HistoryStorage
        {
            public LinkedList<double> Next => _listNext;
            public LinkedList<double> NextMax => _listNextMax;
            public LinkedList<double> NextMinMax => _listNextMinMax;
            public LinkedList<double> NextDouble => _listNextDouble;
            public LinkedList<double> NextBytes => _listNextBytes;

            public HistoryStorageTester(int intSamples = DEFAULT_SAMPLES_COUNT, int doubleSamples = DEFAULT_SAMPLES_COUNT, int byteSamples = DEFAULT_SAMPLES_COUNT)
                : base(GetConfigurationReader(intSamples, doubleSamples, byteSamples))
            {
            }
        }

        private static IConfigurationReader GetConfigurationReader(int intSamples, int doubleSamples, int byteSamples)
        {
            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns
            (
                new RandomSimulationConfiguration
                (
                    ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                    ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                    TasksConfigurationTests.CreateCorrectConfiguration(),
                    new HistoryConfiguration(intSamples, doubleSamples, byteSamples)
                )
            );

            return configurationReaderMock.Object;
        }

        #region AddNotFull

        [Test]
        public void AddNotFullNext()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            int v1 = _random.Next();
            int v2 = _random.Next();
            int v3 = _random.Next();

            tester.StoreNext(v1);
            tester.StoreNext(v2);
            tester.StoreNext(v3);

            Assert.That(tester.Next.Count, Is.EqualTo(3));
            Assert.That(tester.NextMax.Count, Is.Zero);
            Assert.That(tester.NextMinMax.Count, Is.Zero);
            Assert.That(tester.NextDouble.Count, Is.Zero);
            Assert.That(tester.NextBytes.Count, Is.Zero);

            Assert.That(new double[] {v1, v2, v3}.SequenceEqual(tester.Next.ToArray()));
        }

        [Test]
        public void AddNotFullNextMax()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            int v1 = _random.Next();
            int v2 = _random.Next();
            int v3 = _random.Next();

            tester.StoreNextMax(v1);
            tester.StoreNextMax(v2);
            tester.StoreNextMax(v3);

            Assert.That(tester.Next.Count, Is.Zero);
            Assert.That(tester.NextMax.Count, Is.EqualTo(3));
            Assert.That(tester.NextMinMax.Count, Is.Zero);
            Assert.That(tester.NextDouble.Count, Is.Zero);
            Assert.That(tester.NextBytes.Count, Is.Zero);

            Assert.That(new double[] {v1, v2, v3}.SequenceEqual(tester.NextMax.ToArray()));
        }

        [Test]
        public void AddNotFullNextMinMax()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            int v1 = _random.Next();
            int v2 = _random.Next();
            int v3 = _random.Next();

            tester.StoreNextMinMax(v1);
            tester.StoreNextMinMax(v2);
            tester.StoreNextMinMax(v3);

            Assert.That(tester.Next.Count, Is.Zero);
            Assert.That(tester.NextMax.Count, Is.Zero);
            Assert.That(tester.NextMinMax.Count, Is.EqualTo(3));
            Assert.That(tester.NextDouble.Count, Is.Zero);
            Assert.That(tester.NextBytes.Count, Is.Zero);

            Assert.That(new double[] { v1, v2, v3 }.SequenceEqual(tester.NextMinMax.ToArray()));
        }

        [Test]
        public void AddNotFullNextDouble()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            double v1 = _random.NextDouble();
            double v2 = _random.NextDouble();
            double v3 = _random.NextDouble();

            tester.StoreNextDouble(v1);
            tester.StoreNextDouble(v2);
            tester.StoreNextDouble(v3);

            Assert.That(tester.Next.Count, Is.Zero);
            Assert.That(tester.NextMax.Count, Is.Zero);
            Assert.That(tester.NextMinMax.Count, Is.Zero);
            Assert.That(tester.NextDouble.Count, Is.EqualTo(3));
            Assert.That(tester.NextBytes.Count, Is.Zero);

            Assert.That(new[] {v1, v2, v3}.SequenceEqual(tester.NextDouble.ToArray()));
        }

        [Test]
        public void AddNotFullNextBytes()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            byte v1 = Convert.ToByte(_random.Next(0, 256));
            byte v2 = Convert.ToByte(_random.Next(0, 256));
            byte v3 = Convert.ToByte(_random.Next(0, 256));

            tester.StoreNextBytes(new[] {v1});
            tester.StoreNextBytes(new[] {v2, v3});

            Assert.That(tester.Next.Count, Is.Zero);
            Assert.That(tester.NextMax.Count, Is.Zero);
            Assert.That(tester.NextMinMax.Count, Is.Zero);
            Assert.That(tester.NextDouble.Count, Is.Zero);
            Assert.That(tester.NextBytes.Count, Is.EqualTo(3));

            Assert.That(new double[] {v1, v2, v3}.SequenceEqual(tester.NextBytes.ToArray()));
        }

        #endregion AddNotFull

        [Test]
        public void AddFull()
        {
            HistoryStorageTester tester = new HistoryStorageTester();

            const int OVERHEAD = 3;

            List<double> expectedNext = new List<double>(DEFAULT_SAMPLES_COUNT + OVERHEAD);
            List<double> expectedNextMax = new List<double>(DEFAULT_SAMPLES_COUNT + OVERHEAD);
            List<double> expectedNextMinMax = new List<double>(DEFAULT_SAMPLES_COUNT + OVERHEAD);
            List<double> expectedNextDouble = new List<double>(DEFAULT_SAMPLES_COUNT + OVERHEAD);
            List<double> expectedNextBytes = new List<double>(DEFAULT_SAMPLES_COUNT + OVERHEAD);

            for (int i = 0; i < DEFAULT_SAMPLES_COUNT + OVERHEAD; i++)
            {
                int next = _random.Next();
                expectedNext.Add(next);
                tester.StoreNext(next);

                int nextMax = _random.Next();
                expectedNextMax.Add(nextMax);
                tester.StoreNextMax(nextMax);

                int nextMinMax = _random.Next();
                expectedNextMinMax.Add(nextMinMax);
                tester.StoreNextMinMax(nextMinMax);

                double nextDouble = _random.NextDouble();
                expectedNextDouble.Add(nextDouble);
                tester.StoreNextDouble(nextDouble);

                byte nextByte = Convert.ToByte(_random.Next(0, 256));
                expectedNextBytes.Add(nextByte);
                tester.StoreNextBytes(new[] {nextByte});
            }

            Assert.That(tester.Next.Count, Is.EqualTo(DEFAULT_SAMPLES_COUNT));
            Assert.That(tester.NextMax.Count, Is.EqualTo(DEFAULT_SAMPLES_COUNT));
            Assert.That(tester.NextMinMax.Count, Is.EqualTo(DEFAULT_SAMPLES_COUNT));
            Assert.That(tester.NextDouble.Count, Is.EqualTo(DEFAULT_SAMPLES_COUNT));
            Assert.That(tester.NextBytes.Count, Is.EqualTo(DEFAULT_SAMPLES_COUNT));

            Assert.That(expectedNext.Skip(OVERHEAD).SequenceEqual(tester.Next.ToArray()));
            Assert.That(expectedNextMax.Skip(OVERHEAD).SequenceEqual(tester.NextMax.ToArray()));
            Assert.That(expectedNextMinMax.Skip(OVERHEAD).SequenceEqual(tester.NextMinMax.ToArray()));
            Assert.That(expectedNextDouble.Skip(OVERHEAD).SequenceEqual(tester.NextDouble.ToArray()));
            Assert.That(expectedNextBytes.Skip(OVERHEAD).SequenceEqual(tester.NextBytes.ToArray()));
        }
    }
}
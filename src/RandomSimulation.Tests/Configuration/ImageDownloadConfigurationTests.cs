using System;
using NUnit.Framework;
using RandomSimulationEngine.Configuration.ImageDownload;

namespace RandomSimulation.Tests.Configuration
{
    [TestFixture]
    public class ImageDownloadConfigurationTests
    {
        private static readonly string[] _urls = {"wp.pl"};

        private static ImageDownloadConfiguration CreateConfiguration
        (
            string[] frameGrabUrls,
            int downloadIntervalS = 90,
            int oneImageHashCount = 5,
            int taskBytesCacheCapacity = 500
        )
        {
            return new ImageDownloadConfiguration(frameGrabUrls, downloadIntervalS, oneImageHashCount, taskBytesCacheCapacity);
        }

        public static ImageDownloadConfiguration CreateCorrectConfiguration(string[]? urlsOverload = null, int downloadIntervalOverride = 90)
        {
            return CreateConfiguration(urlsOverload ?? _urls, downloadIntervalOverride);
        }

        [Test]
        public void ValidationOk()
        {
            Assert.DoesNotThrow(() => CreateConfiguration(_urls));
        }

        [Test]
        public void Validation_MinimalValues()
        {
            Assert.DoesNotThrow(() => CreateConfiguration(_urls, Convert.ToInt32(ImageDownloadConfiguration.MinimumDownloadInterval.TotalSeconds), 1, 1));
        }

        [Test]
        public void ValidationError_EmptyUrls()
        {
            Assert.Throws<ArgumentException>(() => CreateConfiguration(Array.Empty<string>()));
        }

        [Test]
        public void ValidationError_DownloadInterval()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(_urls, downloadIntervalS: Convert.ToInt32(ImageDownloadConfiguration.MinimumDownloadInterval.TotalSeconds - 1)));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ValidationError_DownloadInterval(int oneImageHashCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(_urls, oneImageHashCount: oneImageHashCount));
        }

        [TestCase(-1)]
        [TestCase(0)]
        public void ValidationError_TaskBytesCacheCapacity(int taskBytesCacheCapacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => CreateConfiguration(_urls, taskBytesCacheCapacity: taskBytesCacheCapacity));
        }
    }
}
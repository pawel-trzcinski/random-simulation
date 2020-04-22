using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks.Specific;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    [TestFixture]
    public class ImageDownloadTaskTests
    {
        private class ImageDownloadTaskTester : ImageDownloadTask
        {
            public ImageDownloadTaskTester(IWebClientWrapper webClientWrapper, IRandomService randomService, IConfigurationReader configurationReader)
                : base(webClientWrapper, randomService, configurationReader, "WHATEVER")
            {
            }
        }

        [Test]
        public void TaskDownloadsImagesAndCreatesRandomBytes()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            int bytesPullCount = 0;

            Mock<IWebClientWrapper> webClientWrapperMock = new Mock<IWebClientWrapper>();
            webClientWrapperMock.Setup(p => p.GetImageBytes(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(() => Helper.GetRandomArray(5000));

            bool randomSeedSet = false;
            Mock<IRandomService> randomServiceMock = new Mock<IRandomService>();
            randomServiceMock.Setup(p => p.Next()).Returns(1);
            randomServiceMock.Setup(p => p.Next(It.IsAny<int>())).Returns(1);
            randomServiceMock.Setup(p => p.SetSeed(It.IsAny<int>())).Callback<int>(i => randomSeedSet = true);

            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(downloadIntervalOverride: 2),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            ImageDownloadTaskTester tester = new ImageDownloadTaskTester(webClientWrapperMock.Object, randomServiceMock.Object, configurationReaderMock.Object);
            tester.ExecutionFinished += (sender, args) =>
            {
                ++bytesPullCount;

                if (bytesPullCount == 2)
                {
                    source.Cancel();
                }
            };

            Assert.IsFalse(tester.IsDataAvailable);
            Assert.IsNull(tester.GetBytes(20));

            tester.Start(source.Token);

            source.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(3600));

            Assert.IsTrue(tester.IsDataAvailable);
            Assert.IsTrue(randomSeedSet);
            Assert.IsFalse(tester.IsRunning);

            byte[] bytes = tester.GetBytes(20);
            Assert.IsNotNull(bytes);
            Assert.AreEqual(20, bytes.Length);
        }
    }
}
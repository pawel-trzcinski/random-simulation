using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RandomSimulationEngine.Extensions;

namespace RandomSimulation.Tests.Extensions
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        private static readonly TimeSpan _safetyTestTime = TimeSpan.FromSeconds(10);

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(127)]
        [TestCase(173)]
        [TestCase(585)]
        [TestCase(1025)]
        [TestCase(10000)]
        public void AllDataIsCopied(int dataLength)
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (MemoryStream inputStream = new MemoryStream(inputArray, 0, dataLength))
                {
                    inputStream.CopyTo(outputStream, CancellationToken.None);
                }

                byte[] outputArray = outputStream.ToArray();

                Assert.True(inputArray.SequenceEqual(outputArray));
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(127)]
        [TestCase(173)]
        [TestCase(585)]
        [TestCase(1025)]
        [TestCase(10000)]
        public void CancellationTokenBreaksAtBegenning(int dataLength)
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                CancellationToken token = source.Token;

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (MemoryStream inputStream = new MemoryStream(inputArray, 0, dataLength))
                    {
                        source.Cancel();
                        inputStream.CopyTo(outputStream, token);
                    }

                    byte[] outputArray = outputStream.ToArray();

                    Assert.Zero(outputArray.Length);
                }
            }
        }

        [TestCase(StreamExtensions.COPY_BUFFER_SIZE * 2)]
        [TestCase(StreamExtensions.COPY_BUFFER_SIZE * 3)]
        [TestCase(StreamExtensions.COPY_BUFFER_SIZE * 5)]
        [TestCase(StreamExtensions.COPY_BUFFER_SIZE * 10)]
        [TestCase(StreamExtensions.COPY_BUFFER_SIZE * 100)]
        public void CancellationTokenBreaksInTheMiddle(int dataLength)
        {
            byte[] inputArray = Helper.GetRandomArray(dataLength);
            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                CancellationToken token = source.Token;

                using (ManualResetEvent notificationHandle = new ManualResetEvent(false))
                using (ManualResetEvent sleepHandle = new ManualResetEvent(false))
                {
                    ManualResetEvent notificationHandle1 = notificationHandle;
                    ManualResetEvent sleepHandle1 = sleepHandle;

                    Task<byte[]> task = Task.Run(() =>
                    {
                        using (MemoryStreamTester outputStream = new MemoryStreamTester(notificationHandle1, sleepHandle1))
                        {
                            using (MemoryStream inputStream = new MemoryStream(inputArray, 0, dataLength))
                            {
                                inputStream.CopyTo(outputStream, token);
                            }

                            return outputStream.ToArray();
                        }
                    }, source.Token);

                    notificationHandle.WaitOne(_safetyTestTime);
                    source.Cancel();
                    sleepHandle.Set();

                    task.Wait(_safetyTestTime);

                    Assert.True(task.Result.Length < dataLength);
                    Assert.True(inputArray.Take(task.Result.Length).SequenceEqual(task.Result));
                }
            }
        }

        private class MemoryStreamTester : MemoryStream
        {
            private readonly ManualResetEvent _notificationHandle;
            private readonly WaitHandle _sleepHandle;

            public MemoryStreamTester(ManualResetEvent notificationHandle, WaitHandle sleepHandle)
            {
                _notificationHandle = notificationHandle;
                _sleepHandle = sleepHandle;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                base.Write(buffer, offset, count);

                _notificationHandle.Set();
                _sleepHandle.WaitOne(_safetyTestTime);
            }
        }
    }
}
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Structures;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulationEngine.Tasks.Specific
{
    public class ImageDownloadTask : ISourceTask
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ImageDownloadTask));

        private const int MINIMUM_NUMBER_OF_BYTES_TO_HASH = 32;

        private readonly IWebClientWrapper _webClientWrapper;
        private readonly IRandomService _randomService;
        private readonly IConfigurationReader _configurationReader;

        public event EventHandler? ExecutionFinished;

        private readonly string _url;

        private readonly AutoResetEvent _resetEvent = new(false);
        private readonly object _executionLockObject = new();

        public bool IsDataAvailable => _queue.Count > 0;

        private volatile bool _isRunning;
        public bool IsRunning => _isRunning;

        private byte[]? _previousImage;
        private readonly ConcurrentLimitedByteQueue _queue;

        private Task? _mainTask;

        public bool IsAlive => _mainTask?.Status == TaskStatus.Running;

        public ImageDownloadTask(IWebClientWrapper webClientWrapper, IRandomService randomService, IConfigurationReader configurationReader, string url)
        {
            _webClientWrapper = webClientWrapper;
            _randomService = randomService;
            _configurationReader = configurationReader;
            _url = url;
            _queue = new ConcurrentLimitedByteQueue(configurationReader.Configuration.ImageDownload.TaskBytesCacheCapacity);
        }

        public void Start(CancellationToken cancellationToken)
        {
            _mainTask = Task.Run(() => TaskThread(cancellationToken), cancellationToken);
        }

        private void TaskThread(CancellationToken cancellationToken)
        {
            try
            {
                TimeSpan initialDelay = TimeSpan.FromSeconds(_randomService.Next(Convert.ToInt32(_configurationReader.Configuration.ImageDownload.DownloadInterval.TotalSeconds)));
                _log.Debug($"Initial delay for {_url} is {initialDelay.TotalSeconds:0}s");

                cancellationToken.Register(() => _resetEvent.Set());

                _resetEvent.WaitOne(initialDelay);

                TimeSpan originalInterval = _configurationReader.Configuration.ImageDownload.DownloadInterval;

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    TimeSpan currentInterval = originalInterval;
                    lock (_executionLockObject)
                    {
                        try
                        {
                            _isRunning = true;
                            _log.Debug($"Downloading from {_url}");
                            byte[] bytes = _webClientWrapper.GetImageBytes(_url, cancellationToken).Result;
                            if (bytes.Length == 0)
                            {
                                throw new InvalidOperationException($"Not enoug bytes received ({bytes.Length})");
                            }

                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            byte[] noise = GetNoise(bytes);

                            // remove previous bytes from memory
                            for (int i = 0; i < _previousImage!.Length; i++)
                            {
                                _previousImage[i] = 0;
                            }

                            _previousImage = bytes;

                            ConsumeBytes(new ReadOnlySpan<byte>(noise));

                            // After every download, set random seed. Random service will protect itself from frequency overflow
                            if (_queue.TryFetch(4, out byte[] randomSeed))
                            {
                                _randomService.SetSeed(BitConverter.ToInt32(randomSeed, 0));
                            }

                            // remove noise from memory
                            for (int i = 0; i < noise.Length; i++)
                            {
                                noise[i] = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);

                            currentInterval += originalInterval;
                        }
                    }

                    _isRunning = false;
                    ExecutionFinished?.Invoke(this, EventArgs.Empty);

                    _resetEvent.WaitOne(currentInterval);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
        }

        #region ConsumeBytes

        private byte[] GetNoise(byte[] currentImage)
        {
            int resultCount = Math.Min(currentImage.Length, (_previousImage ?? new byte[0]).Length);

            if (_previousImage == null)
            {
                _previousImage = new byte[currentImage.Length];
            }
            else
            {
                // if no changes are detected, then throw
                bool changeDetected = false;
                for (int i = 0; i < resultCount; i++)
                {
                    if (currentImage[i] != _previousImage[i])
                    {
                        changeDetected = true;
                        break;
                    }
                }

                if (!changeDetected)
                {
                    throw new InvalidOperationException("No image change detected. Current and previous are the same.");
                }
            }

            byte[] noise = new byte[resultCount];
            for (int i = 0; i < resultCount; i++)
            {
                noise[i] = currentImage[i];
                noise[i] ^= _previousImage[i];
            }

            return noise;
        }

        private void ConsumeBytes(ReadOnlySpan<byte> bytes)
        {
            int oneImageHashCount = _configurationReader.Configuration.ImageDownload.OneImageHashCount;

            // check if it is possible to get number of hashes we want
            int singleHashSourceLength = bytes.Length / oneImageHashCount;
            if (singleHashSourceLength < MINIMUM_NUMBER_OF_BYTES_TO_HASH)
            {
                oneImageHashCount = bytes.Length / MINIMUM_NUMBER_OF_BYTES_TO_HASH;
                singleHashSourceLength = MINIMUM_NUMBER_OF_BYTES_TO_HASH;
            }

            _log.Debug($"singleHashSourceLength: {singleHashSourceLength}");
            _log.Debug($"oneImageHashCount: {oneImageHashCount}");

            for (int i = 0; i < oneImageHashCount; i++)
            {
                ConsumeBytesForOneHash(bytes.Slice(i * singleHashSourceLength, singleHashSourceLength));
            }
        }

        private void ConsumeBytesForOneHash(ReadOnlySpan<byte> span)
        {
            byte[] hash;
            using (SHA512 sha = SHA512.Create())
            {
                hash = sha.ComputeHash(span.ToArray());
            }

            _queue.Enqueue(hash);
            for (int i = 0; i < hash.Length; i++) // clear from memory
            {
                hash[i] = 0;
            }
        }

        #endregion consumeBytes

        public void Poke()
        {
            _log.Debug($"Task poked - {_url}");
            _resetEvent.Set();
        }

        public BytesProvidingResult GetBytes(int count)
        {
            int queueCount = _queue.Count;
            if (queueCount < count)
            {
                _log.Debug($"Not enough data in queue. Needed {count}. Adtual: {queueCount}");
                return BytesProvidingResult.Empty();
            }

            return _queue.TryFetch(count, out byte[] result)
                ? BytesProvidingResult.Create(result)
                : BytesProvidingResult.Empty();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace RandomSimulationEngine.Configuration.ImageDownload
{
    [JsonObject(nameof(RandomSimulationConfiguration.ImageDownload))]
    public class ImageDownloadConfiguration
    {
        public static readonly TimeSpan MinimumDownloadInterval = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Gets collection of addresses from which to download images from
        /// </summary>
        public IReadOnlyCollection<string> FrameGrabUrls { get; }

        public TimeSpan DownloadInterval { get; }

        public int OneImageHashCount { get; }

        public int TaskBytesCacheCapacity { get; }

        [JsonConstructor]
        public ImageDownloadConfiguration([NotNull] string[] frameGrabUrls, int downloadIntervalS, int oneImageHashCount, int taskBytesCacheCapacity)
        {
            if (frameGrabUrls == null)
            {
                throw new ArgumentNullException(nameof(frameGrabUrls));
            }

            this.FrameGrabUrls = new ReadOnlyCollection<string>(frameGrabUrls.ToList());
            this.DownloadInterval = TimeSpan.FromSeconds(downloadIntervalS);
            this.OneImageHashCount = oneImageHashCount;
            this.TaskBytesCacheCapacity = taskBytesCacheCapacity;
            ValidateConfiguration();
        }

        public void ValidateConfiguration()
        {
            if (this.FrameGrabUrls.Count <= 0)
            {
                throw new ArgumentException($"No {nameof(FrameGrabUrls)} defined", nameof(FrameGrabUrls));
            }

            if (DownloadInterval < MinimumDownloadInterval)
            {
                throw new ArgumentOutOfRangeException(nameof(DownloadInterval), $"{nameof(DownloadInterval)} has to be {MinimumDownloadInterval.TotalSeconds:0}s minimum");
            }

            if (OneImageHashCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(OneImageHashCount), $"{nameof(OneImageHashCount)} must be a positive number");
            }

            if (TaskBytesCacheCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(TaskBytesCacheCapacity), $"{nameof(TaskBytesCacheCapacity)} must be a positive number");
            }
        }
    }
}
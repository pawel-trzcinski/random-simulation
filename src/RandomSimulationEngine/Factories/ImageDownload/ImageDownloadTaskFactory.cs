using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks;
using RandomSimulationEngine.Tasks.Specific;
using RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper;

namespace RandomSimulationEngine.Factories.ImageDownload
{
    public class ImageDownloadTaskFactory : IImageDownloadTaskFactory
    {
        private readonly IRandomService _randomService;
        private readonly IConfigurationReader _configurationReader;

        public ImageDownloadTaskFactory(IRandomService randomService, IConfigurationReader configurationReader)
        {
            _randomService = randomService;
            _configurationReader = configurationReader;
        }

        public ISourceTask GetNewTask(string url)
        {
            return new ImageDownloadTask(new WebClientWrapper(), _randomService, _configurationReader, url);
        }
    }
}
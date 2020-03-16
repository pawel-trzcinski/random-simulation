using System.Threading;

namespace RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper
{
    public interface IWebClientWrapper
    {
        byte[] GetImageBytes(string sourceUrl, CancellationToken cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper
{
    public interface IWebClientWrapper
    {
        Task<byte[]> GetImageBytes(string sourceUrl, CancellationToken cancellationToken);
    }
}
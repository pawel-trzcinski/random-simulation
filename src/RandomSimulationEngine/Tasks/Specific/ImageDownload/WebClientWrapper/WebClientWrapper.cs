using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper
{
    public class WebClientWrapper : IWebClientWrapper
    {
        public async Task<byte[]> GetImageBytes(string sourceUrl, CancellationToken cancellationToken)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(sourceUrl, cancellationToken);
            }
        }
    }
}
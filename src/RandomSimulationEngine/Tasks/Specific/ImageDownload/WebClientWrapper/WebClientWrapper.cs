using System.IO;
using System.Net;
using System.Threading;
using RandomSimulationEngine.Extensions;

namespace RandomSimulationEngine.Tasks.Specific.ImageDownload.WebClientWrapper
{
    public class WebClientWrapper : IWebClientWrapper
    {
#warning TODO - unit tests
        public byte[] GetImageBytes(string sourceUrl, CancellationToken cancellationToken)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(sourceUrl);

            using (WebResponse resp = req.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms, cancellationToken);
                    return ms.ToArray();
                }
            }
        }
    }
}
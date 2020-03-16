using System;
using JetBrains.Annotations;
using System.IO;
using System.Threading;

namespace RandomSimulationEngine.Extensions
{
    public static class StreamExtensions
    {
#warning TODO - unit tests
        public static void CopyTo(this Stream source, [NotNull] Stream destination, CancellationToken cancellationToken)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            byte[] buffer = new byte[4 * 1024];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                destination.Write(buffer, 0, read);
            }
        }
    }
}
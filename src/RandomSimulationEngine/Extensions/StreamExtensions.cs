using System;
using JetBrains.Annotations;
using System.IO;
using System.Threading;

namespace RandomSimulationEngine.Extensions
{
    public static class StreamExtensions
    {
        public const int COPY_BUFFER_SIZE = 4 * 1024;

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

            byte[] buffer = new byte[COPY_BUFFER_SIZE];
            int read;
            while ((read = source.Read(buffer, 0, COPY_BUFFER_SIZE)) > 0)
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomSimulation.Tests
{
    public static class Helper
    {
        public static byte[] GetRandomArray(int dataLength)
        {
            int guidLength = Guid.NewGuid().ToByteArray().Length;
            int guidsCount = dataLength / guidLength + 1;

            List<byte> input = new List<byte>(guidsCount * guidLength);
            for (int i = 0; i < guidsCount; i++)
            {
                input.AddRange(Guid.NewGuid().ToByteArray());
            }

            return input.Take(dataLength).ToArray();
        }

    }
}
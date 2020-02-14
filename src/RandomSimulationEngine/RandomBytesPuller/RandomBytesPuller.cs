using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSimulationEngine.RandomBytesPuller
{
    public class RandomBytesPuller : IRandomBytesPuller
    {
        public byte[] Pull(int count)
        {
            // check, że ilość ma być od 1 do 50 (konfigurowalne)

#warning TODO
            return new byte[0];
        }
    }
}

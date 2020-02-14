using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSimulationEngine.ValueCalculator
{
    public class ValueCalculator : IValueCalculator
    {
        public double GetDouble(byte[] bytes)
        {
            // 8bytes check
#warning TODO

            throw new NotImplementedException();
        }

        public int GetInt32(byte[] bytes)
        {
            // 4bytes check
#warning TODO
            throw new NotImplementedException();
        }

        public int GetInt32(byte[] bytes, int min)
        {
            // 4bytes check
#warning TODO
            throw new NotImplementedException();
        }

        public int GetInt32(byte[] bytes, int min, int max)
        {
            // 4bytes check
            throw new NotImplementedException();
        }

        private static int Normalize(int value, int min, int max)
        {
#warning TODO
            return 0;
        }
    }
}

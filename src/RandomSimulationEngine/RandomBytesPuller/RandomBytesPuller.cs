using System.Collections.Generic;
using System.Linq;
using RandomSimulationEngine.Exceptions;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine.RandomBytesPuller
{
    public class RandomBytesPuller : IRandomBytesPuller
    {
#warning TODO - unit tests

        private readonly IRandomService _randomService;

        private readonly List<ISingleSourceBytesProvider> _providers = new List<ISingleSourceBytesProvider>(10);

        public RandomBytesPuller(IRandomService randomService)
        {
            _randomService = randomService;
        }

        public void Register(ISingleSourceBytesProvider singleSourceBytesProvider)
        {
            _providers.Add(singleSourceBytesProvider);
        }

        /// <exception cref="NoDataException">Puller is unable to find source with available data and enough bytes to fetch.</exception>
        public byte[] Pull(int count)
        {
#warning TEST
            foreach (ISingleSourceBytesProvider provider in _providers.Where(p => p.IsDataAvailable).OrderBy(p => _randomService.Next()))
            {
                byte[] bytes = provider.GetBytes(count);

                if (bytes != null)
                {
                    return bytes;
                }
            }

            throw new NoDataException();
        }
    }
}
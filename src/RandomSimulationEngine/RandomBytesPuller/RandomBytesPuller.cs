using System.Collections.Generic;
using System.Linq;
using RandomSimulationEngine.Exceptions;
using RandomSimulationEngine.Random;
using RandomSimulationEngine.Tasks;

namespace RandomSimulationEngine.RandomBytesPuller
{
    public class RandomBytesPuller : IRandomBytesPuller
    {
        private readonly IRandomService _randomService;

        private readonly List<ISingleSourceBytesProvider> _providers = new(10);

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
            foreach (ISingleSourceBytesProvider provider in _providers.Where(p => p.IsDataAvailable).OrderBy(_ => _randomService.Next()))
            {
                BytesProvidingResult result = provider.GetBytes(count);

                if (result.IsDtataAvailable)
                {
                    return result.Data.ToArray();
                }
            }

            throw new NoDataException();
        }
    }
}
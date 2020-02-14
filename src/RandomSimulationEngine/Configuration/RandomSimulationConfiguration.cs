using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace RandomSimulationEngine.Configuration
{
    /// <summary>
    /// Main configuration class for all ClassNamer bussiness logic.
    /// </summary>
    public class RandomSimulationConfiguration
    {
        public ThrottlingConfiguration Throttling { get; }

        /// <summary>
        /// Gets collection of possible <see cref="NamePart"/> combinations along with weight associated to it.
        /// </summary>
        public IReadOnlyCollection<string> FrameGrabUrls { get; }

        /// <summary>
        /// Gets dictionary of available words to pull from. The words are normalized (first letter is big, the rest are small).
        /// </summary>
        //public IReadOnlyDictionary<NamePart, IReadOnlyCollection<string>> WordsSet { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSimulationConfiguration"/> class.
        /// </summary>
        /// <param name="combinations">Collection of possible <see cref="NamePart"/> combinations along with weight associated to it.</param>
        /// <param name="wordsSet">Dictionary of available words to pull from.</param>
        [JsonConstructor]
        public RandomSimulationConfiguration(ThrottlingConfiguration throttling, string[] frameGrabUrls)//, Dictionary<NamePart, string[]> wordsSet)
        {
#warning TODO
            this.Throttling = throttling;
            this.FrameGrabUrls = new ReadOnlyCollection<string>(frameGrabUrls);
            //this.WordsSet = wordsSet.ToDictionary
            //    (
            //        k => k.Key,
            //        v => (IReadOnlyCollection<string>)new ReadOnlyCollection<string>(v.Value.SelectMany(SplitIntoWords).Select(NormalizeWord).ToArray())
            //    );

            ValidateConfiguration();
        }

        
        private void ValidateConfiguration()
        {
            if(this.FrameGrabUrls.Count <= 0)
            {
                throw new ArgumentException("No FrameGrabUrls defined");
            }

#warning TODO
            //foreach (string word in this.WordsSet.Values.SelectMany(p => p))
            //{
            //    foreach (char wordCharacter in word)
            //    {
            //        if (!allowedCharacters.Contains(wordCharacter))
            //        {
            //            throw new ArgumentException($"Invalid character '{wordCharacter}' in word {word}");
            //        }
            //    }
            //}
        }
    }
}
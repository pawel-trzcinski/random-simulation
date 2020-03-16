using System;
using System.Collections.Generic;

namespace RandomSimulation.Tests.Structures
{
    public class QueueFetchingUseCase : QueueUseCase
    {
        public int FetchCount { get; }
        public byte[] FetchResult { get; }

        public QueueFetchingUseCase(IReadOnlyList<string> inputStrings)
            : this(inputStrings, inputStrings[0], inputStrings[1], inputStrings[2], inputStrings[3], inputStrings[4])
        {
        }

        private QueueFetchingUseCase(IReadOnlyList<string> inputStrings, string currentState, string currentStateIndexes, string fetchCount, string expectedStateIndexes, string fetchResult)
            : base(inputStrings, currentState, currentStateIndexes, expectedStateIndexes)
        {
            FetchCount = Convert.ToInt32(fetchCount);
            FetchResult = ConvertStringToState(fetchResult);
        }
    }
}
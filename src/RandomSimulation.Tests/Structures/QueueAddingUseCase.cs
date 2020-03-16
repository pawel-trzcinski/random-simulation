using System.Collections.Generic;

namespace RandomSimulation.Tests.Structures
{
    public class QueueAddingUseCase : QueueUseCase
    {
        public byte[] BufferAdded { get; }
        public byte[] ExpectedState { get; }

        public QueueAddingUseCase(IReadOnlyList<string> inputStrings)
            : this(inputStrings, inputStrings[0], inputStrings[1], inputStrings[2], inputStrings[3], inputStrings[4])
        {
            BufferAdded = ConvertStringToState(inputStrings[2]);
            ExpectedState = ConvertStringToState(inputStrings[3]);
        }

        private QueueAddingUseCase(IReadOnlyList<string> inputStrings, string currentState, string currentStateIndexes, string bufferAdded, string expectedState, string expectedStateIndexes)
            : base(inputStrings, currentState, currentStateIndexes, expectedStateIndexes)
        {
            BufferAdded = ConvertStringToState(bufferAdded);
            ExpectedState = ConvertStringToState(expectedState);
        }
    }
}
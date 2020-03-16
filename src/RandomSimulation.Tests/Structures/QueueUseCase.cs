using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomSimulation.Tests.Structures
{
    public abstract class QueueUseCase
    {
        private readonly string _representation;

        private readonly byte[] _currentState;
        private readonly int _currentFirstIndex;
        private readonly int _currentLastIndex;

        public int ExpectedFirstIndex { get; }
        public int ExpectedLastIndex { get; }

        protected QueueUseCase(IReadOnlyList<string> inputStrings, string currentState, string currentStateIndexes, string expectedStateIndexes)
        {
            StringBuilder sb = new StringBuilder(inputStrings.Sum(p => p.Length) + (inputStrings.Count - 1) * Environment.NewLine.Length);

            sb.Append(inputStrings[0]);
            for (int i = 1; i < inputStrings.Count; i++)
            {
                sb.Append(Environment.NewLine);
                sb.Append(inputStrings[i]);
            }

            _representation = sb.ToString();

            _currentState = ConvertStringToState(currentState);
            (_currentFirstIndex, _currentLastIndex) = ConvertStringToIndexes(currentStateIndexes);
            (ExpectedFirstIndex, ExpectedLastIndex) = ConvertStringToIndexes(expectedStateIndexes);
        }

        public override string ToString()
        {
            return _representation;
        }

        protected static byte[] ConvertStringToState(string inputString)
        {
            List<byte> list = new List<byte>(inputString.Length);
            for (int i = 0; i < inputString.Length; i++)
            {
                char templateChar = inputString[i];
                list.Add(templateChar == ConcurrentLimitedByteQueueTests.NOTHING ? (byte) 0 : (byte) templateChar);
            }

            return list.ToArray();
        }

        private static (int firstIndex, int lastIndex) ConvertStringToIndexes(string inputString)
        {
            int first = -1;
            int last = -1;
            for (int i = 0; i < inputString.Length; i++)
            {
                char templateChar = inputString[i];
                if (templateChar == ConcurrentLimitedByteQueueTests.BOTH_INDEXES_INDICATOR)
                {
                    first = last = i;
                    break;
                }

                if (templateChar == ConcurrentLimitedByteQueueTests.FIRST_INDEX_INDICATOR)
                {
                    first = i;
                }
                else if (templateChar == ConcurrentLimitedByteQueueTests.LAST_INDEX_INDICATOR)
                {
                    last = i;
                }

                if (first != -1 && last != -1)
                {
                    break;
                }
            }

            return (first, last);
        }

        public QueueTester CreateQueue()
        {
            return new QueueTester(_currentState, _currentFirstIndex, _currentLastIndex);
        }
    }
}
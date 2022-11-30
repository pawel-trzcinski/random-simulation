using System;
using System.Threading;

namespace RandomSimulationEngine.Structures
{
    public class ConcurrentLimitedByteQueue
    {
        protected readonly byte[] _queue;
        private readonly int _capacity;
        private volatile int _firstIndex = -1;
        private volatile int _lastIndex = -1;
        private volatile int _count;
        private readonly object _lockObject = new object();

        // kolejka, która jest Thread - Safe
        // kolejka do której się wkłąda pojedyncze bajty albo ich array
        // wewnętrzna struktura to Span<byte> i dwa inty: startIndex, length
        // ACHTUNG! - użyć benchmarka w unit teście - tak jak w tym filmiku

        public int Count => _count;
        public int CurrentFirstIndex => _firstIndex;
        public int CurrentLastIndex => _lastIndex;

        protected ConcurrentLimitedByteQueue(byte[] queue, int count, int firstIndex, int lastIndex)
        {
            _queue = queue;
            _capacity = queue.Length;
            _firstIndex = firstIndex;
            _lastIndex = lastIndex;
            _count = count;
        }

        public ConcurrentLimitedByteQueue(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} must be a positive number");
            }

            if (capacity < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Go use single byte. Don't waste out time.");
            }

            _queue = new byte[capacity];
            this._capacity = capacity;
        }

        public void Enqueue(byte[] bytes)
        {
            bool inputCapacityExceeded = bytes.Length > _capacity;
            int inputStartIndex = inputCapacityExceeded ? bytes.Length - _capacity : 0;
            for (int i = inputStartIndex; i < bytes.Length; i++)
            {
                Enqueue(bytes[i]);
            }
        }

        public void Enqueue(byte b)
        {
            lock (_lockObject)
            {
                if (_count == 0)
                {
                    _firstIndex = 0;
                    _lastIndex = 0;
                    _count = 1;
                    _queue[0] = b;
                }
                else if (_count == _capacity)
                {
                    _firstIndex = (_firstIndex + 1) % _capacity;
                    _lastIndex = (_lastIndex + 1) % _capacity;
                    _queue[_lastIndex] = b;
                }
                else
                {
                    _lastIndex = (_lastIndex + 1) % _capacity;
                    _count = Interlocked.Increment(ref _count);
                    _queue[_lastIndex] = b;
                }
            }
        }

        public bool TryFetch(int count, out byte[] bytes)
        {
            lock (_lockObject)
            {
                if (_count < count)
                {
                    bytes = Array.Empty<byte>();
                    return false;
                }

                _count = Interlocked.Add(ref _count, -count);
                Span<byte> span = new Span<byte>(_queue);

                if (_firstIndex <= _lastIndex) // no boundry passed
                {
                    span = span.Slice(_firstIndex, count);
                    _firstIndex = (_firstIndex + count) % _capacity;
                    bytes= span.ToArray();
                    return true;
                }

                // boundry passed
                int countToBorder = _capacity - _firstIndex;
                int firstSpanCount = countToBorder >= count ? count : countToBorder;
                int countLeft = count - firstSpanCount;

                //Span<byte> firstSpan = span.Slice(_firstIndex, firstSpanCount);
                //Span<byte> secondSpan = span.Slice(0, countLeft);

                bytes = new byte[count];
                for (int i = 0; i < firstSpanCount; i++)
                {
                    bytes[i] = _queue[i + _firstIndex];
                }

                for (int i = 0; i < countLeft; i++)
                {
                    bytes[i+ firstSpanCount] = _queue[i];
                }

                _firstIndex = (_firstIndex + count) % _capacity;
                return true;
            }
        }
    }
}
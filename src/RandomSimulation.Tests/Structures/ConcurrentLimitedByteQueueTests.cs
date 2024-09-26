using System;
using System.Linq;
using NUnit.Framework;
using RandomSimulationEngine.Structures;

namespace RandomSimulation.Tests.Structures
{
    [TestFixture]
    public class ConcurrentLimitedByteQueueTests
    {
        public const char FIRST_INDEX_INDICATOR = 'F';
        public const char LAST_INDEX_INDICATOR = 'L';
        public const char BOTH_INDEXES_INDICATOR = 'B';
        public const char NOTHING = '.';

        private static readonly QueueAddingUseCase[] _insertUseCases =
        {
            #region empty initial

            new // 1
            (
                new[]
                {
                    "......",
                    "......",
                    "1",
                    "1.....",
                    "B....."
                }
            ),
            new // less than cap
            (
                new[]
                {
                    "......",
                    "......",
                    "1234",
                    "1234..",
                    "F..L.."
                }
            ),
            new // full cap
            (
                new[]
                {
                    "......",
                    "......",
                    "123456",
                    "123456",
                    "F....L"
                }
            ),
            new // cap exceeded
            (
                new[]
                {
                    "......",
                    "......",
                    "12345678",
                    "345678",
                    "F....L"
                }
            ),
            new // cap double excess
            (
                new[]
                {
                    "......",
                    "......",
                    "1234567891234567",
                    "234567",
                    "F....L"
                }
            ),

            #endregion empty initial

            #region 1 element initial

            new // 1 beginning
            (
                new[]
                {
                    "q.....",
                    "B.....",
                    "1",
                    "q1....",
                    "FL...."
                }
            ),
            new // 1 middle
            (
                new[]
                {
                    "..q...",
                    "..B...",
                    "1",
                    "..q1..",
                    "..FL.."
                }
            ),
            new // 1 end
            (
                new[]
                {
                    ".....q",
                    ".....B",
                    "1",
                    "1....q",
                    "L....F"
                }
            ),
            new // staying in boundries beginning
            (
                new[]
                {
                    "q.....",
                    "B.....",
                    "1234",
                    "q1234.",
                    "F...L."
                }
            ),
            new // staying in boundries middle
            (
                new[]
                {
                    "..q...",
                    "..B...",
                    "12",
                    "..q12.",
                    "..F.L."
                }
            ),
            new // boundry beginning
            (
                new[]
                {
                    "q.....",
                    "B.....",
                    "12345",
                    "q12345",
                    "F....L"
                }
            ),
            new // boundry middle
            (
                new[]
                {
                    "..q...",
                    "..B...",
                    "123",
                    "..q123",
                    "..F..L"
                }
            ),
            new // go pass boundry begenning
            (
                new[]
                {
                    "q.....",
                    "B.....",
                    "12345678",
                    "834567",
                    "LF...."
                }
            ),
            new // go pass boundry middle
            (
                new[]
                {
                    "..q...",
                    "..B...",
                    "123456789zxcvb",
                    "cvb9zx",
                    "..LF.."
                }
            ),
            new // go pass boundry end
            (
                new[]
                {
                    ".....q",
                    ".....B",
                    "1234567",
                    "234567",
                    "F....L"
                }
            ),

            #endregion 1 element initial

            #region partialy empty initial

            new // beginning
            (
                new[]
                {
                    "qwer..",
                    "F..L..",
                    "12",
                    "qwer12",
                    "F....L"
                }
            ),
            new // middle
            (
                new[]
                {
                    ".qwer.",
                    ".F..L.",
                    "1",
                    ".qwer1",
                    ".F...L"
                }
            ),
            new // end
            (
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "123",
                    "123wer",
                    "..LF.."
                }
            ),
            new // staying in boundries beginning
            (
                new[]
                {
                    "qwer..",
                    "F..L..",
                    "12",
                    "qwer12",
                    "F....L"
                }
            ),
            new // staying in boundries middle
            (
                new[]
                {
                    ".qwer.",
                    ".F..L.",
                    "1",
                    ".qwer1",
                    ".F...L"
                }
            ),
            new // boundry beginning
            (
                new[]
                {
                    "qwert.",
                    "F...L.",
                    "1",
                    "qwert1",
                    "F....L"
                }
            ),
            new // boundry middle
            (
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "12",
                    "12qwer",
                    ".LF..."
                }
            ),
            new // go pass boundry begenning
            (
                new[]
                {
                    "qwer..",
                    "F..L..",
                    "1234",
                    "34er12",
                    ".LF..."
                }
            ),
            new // go pass boundry middle
            (
                new[]
                {
                    "..qw..",
                    "..FL..",
                    "123",
                    "3.qw12",
                    "L.F..."
                }
            ),
            new // go pass boundry end
            (
                new[]
                {
                    "w....q",
                    "L....F",
                    "1234567",
                    "723456",
                    "LF...."
                }
            ),

            #endregion partialy empty initial

            #region full initial

            new // beginning
            (
                new[]
                {
                    "qwerty",
                    "F....L",
                    "12",
                    "12erty",
                    ".LF..."
                }
            ),
            new // middle
            (
                new[]
                {
                    "yqwert",
                    "LF....",
                    "123",
                    "y123rt",
                    "...LF."
                }
            ),
            new // end
            (
                new[]
                {
                    "wertyq",
                    "0...LF",
                    "123",
                    "23rty1",
                    ".LF..."
                }
            ),
            new // staying in boundries middle
            (
                new[]
                {
                    "yqwert",
                    "LF....",
                    "12",
                    "y12ert",
                    "..LF.."
                }
            )

            #endregion full initial
        };

        private static readonly QueueFetchingUseCase[] _fetchUseCases =
        {
            #region take one

            // begenning
            new(
                new[]
                {
                    "qw....",
                    "FL....",
                    "1",
                    ".B....",
                    "q"
                }
            ),

            // middle
            new(
                new[]
                {
                    "..qw..",
                    "..FL..",
                    "1",
                    "...B..",
                    "q"
                }
            ),
            new(
                new[]
                {
                    "..qwe.",
                    "..F.L.",
                    "1",
                    "...FL.",
                    "q"
                }
            ),

            // end
            new(
                new[]
                {
                    "wer..q",
                    "..L..F",
                    "1",
                    "F.L...",
                    "q"
                }
            ),


            #endregion take one

            #region take some

            // begenning
            new(
                new[]
                {
                    "qwe...",
                    "F.L...",
                    "2",
                    "..B...",
                    "qw"
                }
            ),

            // middle
            new(
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "3",
                    ".....B",
                    "qwe"
                }
            ),
            new(
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "2",
                    "....FL",
                    "qw"
                }
            ),
            new(
                new[]
                {
                    "rt.qwe",
                    ".L.F..",
                    "3",
                    "FL....",
                    "qwe"
                }
            ),

            //end
            new(
                new[]
                {
                    "we...q",
                    ".L...F",
                    "2",
                    ".B....",
                    "qw"
                }
            ),

            #endregion take some

            #region take all

            // from begenning
            new(
                new[]
                {
                    "q.....",
                    "B.....",
                    "1",
                    "......",
                    "q"
                }
            ),
            new(
                new[]
                {
                    "qwe...",
                    "F.L...",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new(
                new[]
                {
                    "qwerty",
                    "F....L",
                    "6",
                    "......",
                    "qwerty"
                }
            ),

            //from middle
            new(
                new[]
                {
                    "..q...",
                    "..B...",
                    "1",
                    "......",
                    "q"
                }
            ),
            new(
                new[]
                {
                    "..qwe.",
                    "..F.L.",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new(
                new[]
                {
                    "...qwe",
                    "...F.L",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new(
                new[]
                {
                    "rt.qwe",
                    ".L.F..",
                    "5",
                    "......",
                    "qwert"
                }
            ),
            new(
                new[]
                {
                    "rtyqwe",
                    "..LF..",
                    "6",
                    "......",
                    "qwerty"
                }
            ),

            //from end
            new(
                new[]
                {
                    ".....q",
                    ".....B",
                    "1",
                    "......",
                    "q"
                }
            ),
            new(
                new[]
                {
                    "we...q",
                    "L....F",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new(
                new[]
                {
                    "wertyq",
                    "....LF",
                    "6",
                    "......",
                    "qwerty"
                }
            ),

            #endregion take all
        };

        [Test]
        public void AddingCreatesSpecificState([ValueSource(nameof(_insertUseCases))] QueueAddingUseCase useCase)
        {
            Console.WriteLine($"Testing:{Environment.NewLine}{useCase}");

            QueueTester queue = useCase.CreateQueue();
            queue.Enqueue(useCase.BufferAdded);

            Assert.That(useCase.ExpectedState.SequenceEqual(queue.Queue));
            Assert.That(useCase.ExpectedFirstIndex, Is.EqualTo(queue.CurrentFirstIndex));
            Assert.That(useCase.ExpectedLastIndex, Is.EqualTo(queue.CurrentLastIndex));
        }

        [Test]
        public void FetchingFromEmptyQueueFails()
        {
            ConcurrentLimitedByteQueue queue = new ConcurrentLimitedByteQueue(6);
            Assert.That(queue.TryFetch(1, out _), Is.False);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(6)]
        public void FetchingMoreThanIsInQueueFails(int queueCount)
        {
            ConcurrentLimitedByteQueue queue = new ConcurrentLimitedByteQueue(queueCount * 2);

            for (int i = 0; i < queueCount; i++)
            {
                queue.Enqueue(1);
            }

            Assert.That(queue.TryFetch(queueCount + 1, out _), Is.False);
        }

        [Test]
        public void FetchingCreatesSpecificState([ValueSource(nameof(_fetchUseCases))] QueueFetchingUseCase useCase)
        {
            Console.WriteLine($"Testing:{Environment.NewLine}{useCase}");

            QueueTester queue = useCase.CreateQueue();
            int originalQueueLength = queue.Count;
            Assert.That(queue.TryFetch(useCase.FetchCount, out byte[] fetchResult));

            if (queue.Count > 0)
            {
                Assert.That(useCase.ExpectedFirstIndex, Is.EqualTo(queue.CurrentFirstIndex));
                Assert.That(useCase.ExpectedLastIndex, Is.EqualTo(queue.CurrentLastIndex));
            }

            Assert.That(originalQueueLength - useCase.FetchCount, Is.EqualTo(queue.Count));
            Assert.That(useCase.FetchResult.SequenceEqual(fetchResult));
        }
    }
}
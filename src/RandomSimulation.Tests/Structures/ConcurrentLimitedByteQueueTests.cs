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

            new QueueAddingUseCase // 1
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
            new QueueAddingUseCase // less than cap
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
            new QueueAddingUseCase // full cap
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
            new QueueAddingUseCase // cap exceeded
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
            new QueueAddingUseCase // cap double excess
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

            new QueueAddingUseCase // 1 beginning
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
            new QueueAddingUseCase // 1 middle
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
            new QueueAddingUseCase // 1 end
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
            new QueueAddingUseCase // staying in boundries beginning
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
            new QueueAddingUseCase // staying in boundries middle
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
            new QueueAddingUseCase // boundry beginning
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
            new QueueAddingUseCase // boundry middle
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
            new QueueAddingUseCase // go pass boundry begenning
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
            new QueueAddingUseCase // go pass boundry middle
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
            new QueueAddingUseCase // go pass boundry end
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

            new QueueAddingUseCase // beginning
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
            new QueueAddingUseCase // middle
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
            new QueueAddingUseCase // end
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
            new QueueAddingUseCase // staying in boundries beginning
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
            new QueueAddingUseCase // staying in boundries middle
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
            new QueueAddingUseCase // boundry beginning
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
            new QueueAddingUseCase // boundry middle
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
            new QueueAddingUseCase // go pass boundry begenning
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
            new QueueAddingUseCase // go pass boundry middle
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
            new QueueAddingUseCase // go pass boundry end
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

            new QueueAddingUseCase // beginning
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
            new QueueAddingUseCase // middle
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
            new QueueAddingUseCase // end
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
            new QueueAddingUseCase // staying in boundries middle
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
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
                new[]
                {
                    "..qw..",
                    "..FL..",
                    "1",
                    "...B..",
                    "q"
                }
            ),
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "3",
                    ".....B",
                    "qwe"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "..qwer",
                    "..F..L",
                    "2",
                    "....FL",
                    "qw"
                }
            ),
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
                new[]
                {
                    "q.....",
                    "B.....",
                    "1",
                    "......",
                    "q"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "qwe...",
                    "F.L...",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
                new[]
                {
                    "..q...",
                    "..B...",
                    "1",
                    "......",
                    "q"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "..qwe.",
                    "..F.L.",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "...qwe",
                    "...F.L",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "rt.qwe",
                    ".L.F..",
                    "5",
                    "......",
                    "qwert"
                }
            ),
            new QueueFetchingUseCase
            (
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
            new QueueFetchingUseCase
            (
                new[]
                {
                    ".....q",
                    ".....B",
                    "1",
                    "......",
                    "q"
                }
            ),
            new QueueFetchingUseCase
            (
                new[]
                {
                    "we...q",
                    "L....F",
                    "3",
                    "......",
                    "qwe"
                }
            ),
            new QueueFetchingUseCase
            (
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

            Assert.IsTrue(useCase.ExpectedState.SequenceEqual(queue.Queue));
            Assert.AreEqual(useCase.ExpectedFirstIndex, queue.CurrentFirstIndex);
            Assert.AreEqual(useCase.ExpectedLastIndex, queue.CurrentLastIndex);
        }

        [Test]
        public void FetchingFromEmptyQueueFails()
        {
            ConcurrentLimitedByteQueue queue = new ConcurrentLimitedByteQueue(6);
            Assert.IsFalse(queue.TryFetch(1, out _));
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

            Assert.IsFalse(queue.TryFetch(queueCount + 1, out _));
        }

        [Test]
        public void FetchingCreatesSpecificState([ValueSource(nameof(_fetchUseCases))] QueueFetchingUseCase useCase)
        {
            Console.WriteLine($"Testing:{Environment.NewLine}{useCase}");

            QueueTester queue = useCase.CreateQueue();
            int originalQueueLength = queue.Count;
            Assert.IsTrue(queue.TryFetch(useCase.FetchCount, out byte[] fetchResult));

            if (queue.Count > 0)
            {
                Assert.AreEqual(useCase.ExpectedFirstIndex, queue.CurrentFirstIndex);
                Assert.AreEqual(useCase.ExpectedLastIndex, queue.CurrentLastIndex);
            }

            Assert.AreEqual(originalQueueLength - useCase.FetchCount, queue.Count);
            Assert.IsTrue(useCase.FetchResult.SequenceEqual(fetchResult));
        }
    }
}
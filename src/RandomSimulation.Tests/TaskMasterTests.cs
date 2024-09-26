using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.DateTime;
using RandomSimulationEngine.Tasks;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class TaskMasterTests
    {
        private class TaskMasterTester : TaskMaster
        {
            public PokableTaskTester[] Tasks => _tasks.Cast<PokableTaskTester>().ToArray();

            public TaskMasterTester(IConfigurationReader configurationReader, IDateTimeService dateTimeService)
                : base(configurationReader, dateTimeService)
            {
            }

            public void WriteTasksToConsole()
            {
                Console.WriteLine(String.Join(" - ", Tasks.Select(p => p.Id)));
            }
        }

        private class PokableTaskTester : IPokableTask
        {
            public string Id { get; }

            public event EventHandler? ExecutionFinished;
            public bool IsRunning { get; } = false;

            public PokableTaskTester(string id)
            {
                Id = id;
            }

            public void InvokeEvent()
            {
                ExecutionFinished?.Invoke(this, EventArgs.Empty);
            }

            public void Start(CancellationToken cancellationToken)
            {
            }

            public void Poke()
            {
            }
        }

        [Test]
        public void TaskExecutionPutsItFirstInList()
        {
            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, new DateTimeService());

            PokableTaskTester pokableTaskMock1 = new(nameof(pokableTaskMock1));
            PokableTaskTester pokableTaskMock2 = new(nameof(pokableTaskMock2));
            PokableTaskTester pokableTaskMock3 = new(nameof(pokableTaskMock3));
            PokableTaskTester pokableTaskMock4 = new(nameof(pokableTaskMock4));

            tester.Register(pokableTaskMock1);
            tester.Register(pokableTaskMock2);
            tester.Register(pokableTaskMock3);
            tester.Register(pokableTaskMock4);
            tester.WriteTasksToConsole();

            pokableTaskMock2.InvokeEvent();
            tester.WriteTasksToConsole();

            Assert.That(pokableTaskMock2.Id, Is.EqualTo(tester.Tasks[0].Id));
            Assert.That(pokableTaskMock1.Id, Is.EqualTo(tester.Tasks[1].Id));
            Assert.That(pokableTaskMock3.Id, Is.EqualTo(tester.Tasks[2].Id));
            Assert.That(pokableTaskMock4.Id, Is.EqualTo(tester.Tasks[3].Id));

            pokableTaskMock3.InvokeEvent();
            tester.WriteTasksToConsole();

            Assert.That(pokableTaskMock3.Id, Is.EqualTo(tester.Tasks[0].Id));
            Assert.That(pokableTaskMock2.Id, Is.EqualTo(tester.Tasks[1].Id));
            Assert.That(pokableTaskMock1.Id, Is.EqualTo(tester.Tasks[2].Id));
            Assert.That(pokableTaskMock4.Id, Is.EqualTo(tester.Tasks[3].Id));
        }

        [Test]
        public void StartTasksStartsAllTasks()
        {
            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, new DateTimeService());

            int executionsIndicator = 0;
            Mock<IPokableTask> pokableTaskMock1 = new();
            pokableTaskMock1.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => executionsIndicator += 1);
            Mock<IPokableTask> pokableTaskMock2 = new();
            pokableTaskMock2.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => executionsIndicator += 2);
            Mock<IPokableTask> pokableTaskMock3 = new();
            pokableTaskMock3.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => executionsIndicator += 4);
            Mock<IPokableTask> pokableTaskMock4 = new();
            pokableTaskMock4.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => executionsIndicator += 8);

            tester.Register(pokableTaskMock1.Object);
            tester.Register(pokableTaskMock2.Object);
            tester.Register(pokableTaskMock3.Object);
            tester.Register(pokableTaskMock4.Object);

            CancellationTokenSource source = new CancellationTokenSource();

            tester.StartTasks(source.Token);
            source.Cancel();

            Assert.That(executionsIndicator, Is.EqualTo(15));
        }

        [Test]
        public void LongIdleTimeMakesNotRunningTasksPoked()
        {
            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            Queue<DateTime> dateQueue = new Queue<DateTime>(2);
            dateQueue.Enqueue(DateTime.UtcNow);
            dateQueue.Enqueue(DateTime.UtcNow + TimeSpan.FromSeconds(1));
            dateQueue.Enqueue(DateTime.UtcNow + TimeSpan.FromDays(1));

            Mock<IDateTimeService> dateTimeServiceMock = new();
            dateTimeServiceMock.Setup(p => p.UtcNow).Returns(() => dateQueue.Dequeue());

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, dateTimeServiceMock.Object);

            CancellationTokenSource source = new CancellationTokenSource();

            int pokeIndicator = 0;
            Mock<IPokableTask> pokableTaskMock1 = new();
            pokableTaskMock1.Setup(p => p.IsRunning).Returns(false);
            pokableTaskMock1.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 1;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock2 = new();
            pokableTaskMock2.Setup(p => p.IsRunning).Returns(true);
            pokableTaskMock2.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 2;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock3 = new();
            pokableTaskMock3.Setup(p => p.IsRunning).Returns(false);
            pokableTaskMock3.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 4;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock4 = new();
            pokableTaskMock4.Setup(p => p.IsRunning).Returns(true);
            pokableTaskMock4.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 8;
                source.Cancel();
            });

            tester.Register(pokableTaskMock1.Object);
            tester.Register(pokableTaskMock2.Object);
            tester.Register(pokableTaskMock3.Object);
            tester.Register(pokableTaskMock4.Object);

            tester.StartTasks(source.Token);

            source.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(2)); // If it does not happen after 2s, then it's a real problem

            Assert.That(pokeIndicator, Is.EqualTo(4));
        }
    }
}
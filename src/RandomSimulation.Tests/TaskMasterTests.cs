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

            public event EventHandler ExecutionFinished;
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
                TasksConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, new DateTimeService());

            PokableTaskTester pokableTaskMock1 = new PokableTaskTester(nameof(pokableTaskMock1));
            PokableTaskTester pokableTaskMock2 = new PokableTaskTester(nameof(pokableTaskMock2));
            PokableTaskTester pokableTaskMock3 = new PokableTaskTester(nameof(pokableTaskMock3));
            PokableTaskTester pokableTaskMock4 = new PokableTaskTester(nameof(pokableTaskMock4));

            tester.Register(pokableTaskMock1);
            tester.Register(pokableTaskMock2);
            tester.Register(pokableTaskMock3);
            tester.Register(pokableTaskMock4);
            tester.WriteTasksToConsole();

            pokableTaskMock2.InvokeEvent();
            tester.WriteTasksToConsole();

            Assert.AreEqual(pokableTaskMock2.Id, tester.Tasks[0].Id);
            Assert.AreEqual(pokableTaskMock1.Id, tester.Tasks[1].Id);
            Assert.AreEqual(pokableTaskMock3.Id, tester.Tasks[2].Id);
            Assert.AreEqual(pokableTaskMock4.Id, tester.Tasks[3].Id);

            pokableTaskMock3.InvokeEvent();
            tester.WriteTasksToConsole();

            Assert.AreEqual(pokableTaskMock3.Id, tester.Tasks[0].Id);
            Assert.AreEqual(pokableTaskMock2.Id, tester.Tasks[1].Id);
            Assert.AreEqual(pokableTaskMock1.Id, tester.Tasks[2].Id);
            Assert.AreEqual(pokableTaskMock4.Id, tester.Tasks[3].Id);
        }

        [Test]
        public void StartTasksStartsAllTasks()
        {
            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                TasksConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, new DateTimeService());

            int executionsIndicator = 0;
            Mock<IPokableTask> pokableTaskMock1 = new Mock<IPokableTask>();
            pokableTaskMock1.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct => executionsIndicator += 1);
            Mock<IPokableTask> pokableTaskMock2 = new Mock<IPokableTask>();
            pokableTaskMock2.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct => executionsIndicator += 2);
            Mock<IPokableTask> pokableTaskMock3 = new Mock<IPokableTask>();
            pokableTaskMock3.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct => executionsIndicator += 4);
            Mock<IPokableTask> pokableTaskMock4 = new Mock<IPokableTask>();
            pokableTaskMock4.Setup(p => p.Start(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct => executionsIndicator += 8);

            tester.Register(pokableTaskMock1.Object);
            tester.Register(pokableTaskMock2.Object);
            tester.Register(pokableTaskMock3.Object);
            tester.Register(pokableTaskMock4.Object);

            CancellationTokenSource source = new CancellationTokenSource();

            tester.StartTasks(source.Token);
            source.Cancel();

            Assert.AreEqual(15, executionsIndicator);
        }

        [Test]
        public void LongIdleTimeMakesNotRunningTasksPoked()
        {
            var configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(),
                TasksConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            Queue<DateTime> dateQueue = new Queue<DateTime>(2);
            dateQueue.Enqueue(DateTime.UtcNow);
            dateQueue.Enqueue(DateTime.UtcNow + TimeSpan.FromSeconds(1));
            dateQueue.Enqueue(DateTime.UtcNow + TimeSpan.FromDays(1));

            Mock<IDateTimeService> dateTimeServiceMock = new Mock<IDateTimeService>();
            dateTimeServiceMock.Setup(p => p.UtcNow).Returns(() => dateQueue.Dequeue());

            TaskMasterTester tester = new TaskMasterTester(configurationReaderMock.Object, dateTimeServiceMock.Object);

            CancellationTokenSource source = new CancellationTokenSource();

            int pokeIndicator = 0;
            Mock<IPokableTask> pokableTaskMock1 = new Mock<IPokableTask>();
            pokableTaskMock1.Setup(p => p.IsRunning).Returns(false);
            pokableTaskMock1.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 1;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock2 = new Mock<IPokableTask>();
            pokableTaskMock2.Setup(p => p.IsRunning).Returns(true);
            pokableTaskMock2.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 2;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock3 = new Mock<IPokableTask>();
            pokableTaskMock3.Setup(p => p.IsRunning).Returns(false);
            pokableTaskMock3.Setup(p => p.Poke()).Callback(() =>
            {
                pokeIndicator += 4;
                source.Cancel();
            });
            Mock<IPokableTask> pokableTaskMock4 = new Mock<IPokableTask>();
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

            Assert.AreEqual(4, pokeIndicator);
        }
    }
}
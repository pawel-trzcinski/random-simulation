using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moq;
using NUnit.Framework;
using RandomSimulation.Tests.Configuration;
using RandomSimulation.Tests.Tasks.ImageDownload;
using RandomSimulationEngine;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.Factories.ImageDownload;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.RandomBytesPuller;
using RandomSimulationEngine.Tasks;

namespace RandomSimulation.Tests
{
    [TestFixture]
    public class EngineTests
    {
        private class EngineTester : Engine
        {
            private readonly ManualResetEvent _runResetEvent = new(false);
            public ManualResetEvent StartResetEvent { get; } = new(false);

            public EngineTester
            (
                IConfigurationReader configurationReader,
                IControllerFactory controllerFactory,
                IImageDownloadTaskFactory imageDownloadTaskFactory,
                ITaskMaster taskMaster,
                IRandomBytesPuller randomBytesPuller,
                IHealthChecker healthChecker
            )
                : base
                (
                    configurationReader,
                    controllerFactory,
                    imageDownloadTaskFactory,
                    taskMaster,
                    randomBytesPuller,
                    healthChecker
                )
            {
            }

            protected override void StartHosting()
            {
                StartResetEvent.Set();

                _runResetEvent.WaitOne(TimeSpan.FromSeconds(30));
            }

            protected override void StopHosting()
            {
                _runResetEvent.Set();
            }
        }

        [Test]
        public void AllTasksRegistered()
        {
            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();
            Guid g3 = Guid.NewGuid();

            string url1 = g1.ToString();
            string url2 = g2.ToString();
            string url3 = g3.ToString();

            RandomSimulationConfiguration configuration = new RandomSimulationConfiguration
            (
                ThrottlingConfigurationTests.CreateCorrectConfiguration(),
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(new[] { url1, url2, url3 }),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            Mock<IControllerFactory> controllerFactoryMock = new();
            controllerFactoryMock.Setup(p => p.CreateController(It.IsAny<ControllerContext>())).Returns(new TestController());

            Mock<ISourceTask> sourceTaskMock1 = new();
            sourceTaskMock1.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Create(g1.ToByteArray()));
            Mock<ISourceTask> sourceTaskMock2 = new();
            sourceTaskMock2.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Create(g2.ToByteArray()));
            Mock<ISourceTask> sourceTaskMock3 = new();
            sourceTaskMock3.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(BytesProvidingResult.Create(g3.ToByteArray()));

            Mock<IImageDownloadTaskFactory> imageDownloadTaskFactoryMock = new();
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url1)).Returns(sourceTaskMock1.Object);
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url2)).Returns(sourceTaskMock2.Object);
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url3)).Returns(sourceTaskMock3.Object);

            int taskMasterRegistrations = 0;
            Mock<ITaskMaster> taskMasterMock = new();
            taskMasterMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<IPokableTask>(_ => { taskMasterRegistrations += 1; });
            taskMasterMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<IPokableTask>(_ => { taskMasterRegistrations += 2; });
            taskMasterMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<IPokableTask>(_ => { taskMasterRegistrations += 4; });

            bool tasksMasterTasksStarted = false;
            taskMasterMock.Setup(p => p.StartTasks(It.IsAny<CancellationToken>())).Callback<CancellationToken>(_ => { tasksMasterTasksStarted = true; });

            int randomBytesPullerRegistrations = 0;
            Mock<IRandomBytesPuller> randomBytesPullerMock = new();
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<ISingleSourceBytesProvider>(_ => { randomBytesPullerRegistrations += 1; });
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<ISingleSourceBytesProvider>(_ => { randomBytesPullerRegistrations += 2; });
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<ISingleSourceBytesProvider>(_ => { randomBytesPullerRegistrations += 4; });

            int healthCheckerRegistrations = 0;
            Mock<IHealthChecker> healthCheckerMock = new();
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<IHealthProvider>(_ => { healthCheckerRegistrations += 1; });
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<IHealthProvider>(_ => { healthCheckerRegistrations += 2; });
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<IHealthProvider>(_ => { healthCheckerRegistrations += 4; });

            EngineTester engine = new EngineTester
            (
                configurationReaderMock.Object,
                controllerFactoryMock.Object,
                imageDownloadTaskFactoryMock.Object,
                taskMasterMock.Object,
                randomBytesPullerMock.Object,
                healthCheckerMock.Object
            );

            Task engineTask = Task.Run(engine.Start);
            engine.StartResetEvent.WaitOne(TimeSpan.FromSeconds(5));

            Assert.That(taskMasterRegistrations, Is.EqualTo(7));
            Assert.That(randomBytesPullerRegistrations, Is.EqualTo(7));
            Assert.That(healthCheckerRegistrations, Is.EqualTo(7));
            Assert.That(tasksMasterTasksStarted);

            engine.Stop();

            engineTask.Wait(TimeSpan.FromSeconds(10));
            Assert.That(engineTask.Status, Is.Not.EqualTo(TaskStatus.Running));
        }
    }
}

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
            private readonly ManualResetEvent _runResetEvent = new ManualResetEvent(false);
            public ManualResetEvent StartResetEvent { get; } = new ManualResetEvent(false);

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
                ImageDownloadConfigurationTests.CreateCorrectConfiguration(new[] {url1, url2, url3}),
                TasksConfigurationTests.CreateCorrectConfiguration(),
                HistoryConfigurationTests.CreateCorrectConfiguration()
            );

            Mock<IConfigurationReader> configurationReaderMock = new Mock<IConfigurationReader>();
            configurationReaderMock.Setup(p => p.Configuration).Returns(configuration);

            Mock<IControllerFactory> controllerFactoryMock = new Mock<IControllerFactory>();
            controllerFactoryMock.Setup(p => p.CreateController(It.IsAny<ControllerContext>())).Returns(new TestController());

            Mock<ISourceTask> sourceTaskMock1 = new Mock<ISourceTask>();
            sourceTaskMock1.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(g1.ToByteArray());
            Mock<ISourceTask> sourceTaskMock2 = new Mock<ISourceTask>();
            sourceTaskMock2.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(g2.ToByteArray());
            Mock<ISourceTask> sourceTaskMock3 = new Mock<ISourceTask>();
            sourceTaskMock3.Setup(p => p.GetBytes(It.IsAny<int>())).Returns(g3.ToByteArray());

            Mock<IImageDownloadTaskFactory> imageDownloadTaskFactoryMock = new Mock<IImageDownloadTaskFactory>();
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url1)).Returns(sourceTaskMock1.Object);
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url2)).Returns(sourceTaskMock2.Object);
            imageDownloadTaskFactoryMock.Setup(p => p.GetNewTask(url3)).Returns(sourceTaskMock3.Object);

            int taskMasterRegistrations = 0;
            Mock<ITaskMaster> taskMasterMock = new Mock<ITaskMaster>();
            taskMasterMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<IPokableTask>(t => { taskMasterRegistrations += 1; });
            taskMasterMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<IPokableTask>(t => { taskMasterRegistrations += 2; });
            taskMasterMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<IPokableTask>(t => { taskMasterRegistrations += 4; });

            bool tasksMasterTasksStarted = false;
            taskMasterMock.Setup(p => p.StartTasks(It.IsAny<CancellationToken>())).Callback<CancellationToken>(ct => { tasksMasterTasksStarted = true; });

            int randomBytesPullerRegistrations = 0;
            Mock<IRandomBytesPuller> randomBytesPullerMock = new Mock<IRandomBytesPuller>();
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<ISingleSourceBytesProvider>(t => { randomBytesPullerRegistrations += 1; });
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<ISingleSourceBytesProvider>(t => { randomBytesPullerRegistrations += 2; });
            randomBytesPullerMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<ISingleSourceBytesProvider>(t => { randomBytesPullerRegistrations += 4; });

            int healthCheckerRegistrations = 0;
            Mock<IHealthChecker> healthCheckerMock = new Mock<IHealthChecker>();
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock1.Object)).Callback<IHealthProvider>(t => { healthCheckerRegistrations += 1; });
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock2.Object)).Callback<IHealthProvider>(t => { healthCheckerRegistrations += 2; });
            healthCheckerMock.Setup(p => p.Register(sourceTaskMock3.Object)).Callback<IHealthProvider>(t => { healthCheckerRegistrations += 4; });

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

            Assert.AreEqual(7, taskMasterRegistrations);
            Assert.AreEqual(7, randomBytesPullerRegistrations);
            Assert.AreEqual(7, healthCheckerRegistrations);
            Assert.IsTrue(tasksMasterTasksStarted);

            engine.Stop();

            engineTask.Wait(TimeSpan.FromSeconds(10));
            Assert.AreNotEqual(TaskStatus.Running, engineTask.Status);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using RandomSimulationEngine.Configuration;
using RandomSimulationEngine.DateTime;

namespace RandomSimulationEngine.Tasks
{
    public class TaskMaster : ITaskMaster
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TaskMaster));

        private readonly IDateTimeService _dateTimeService;

        protected readonly LinkedList<IPokableTask> _tasks = new LinkedList<IPokableTask>();
        private readonly object _lockObject = new object();

        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _checkAfterPokeInterval = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _errorInterval = TimeSpan.FromSeconds(10);

        private System.DateTime _lastExecutionTime;
        private readonly TimeSpan _idleTimeAllowed;

        public TaskMaster(IConfigurationReader configurationReader, IDateTimeService dateTimeService)
        {
            this._idleTimeAllowed = configurationReader.Configuration.Tasks.IdleTimeAllowed;
            this._dateTimeService = dateTimeService;

            _lastExecutionTime = dateTimeService.UtcNow;
        }

        public void Register(IPokableTask pokableTask)
        {
            pokableTask.ExecutionFinished += PokableTaskOnExecutionFinished;
            lock (_lockObject)
            {
                _tasks.AddLast(pokableTask);
            }
        }

        private void PokableTaskOnExecutionFinished(object? sender, EventArgs e)
        {
            try
            {
                if (sender is IPokableTask pokableTask)
                {
                    lock (_lockObject)
                    {
                        if (!_tasks.Remove(pokableTask))
                        {
                            throw new InvalidOperationException("Task finished was not registered correctly");
                        }

                        _tasks.AddFirst(pokableTask);
                    }

                    _lastExecutionTime = _dateTimeService.UtcNow;
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
        }

        public void StartTasks(CancellationToken cancellationToken)
        {
            IPokableTask[] pokableTasks;
            lock (_lockObject)
            {
                pokableTasks = _tasks.ToArray();
            }

            foreach (IPokableTask task in pokableTasks)
            {
                task.Start(cancellationToken);
            }

            _lastExecutionTime = _dateTimeService.UtcNow;

            Task.Run(() => PokerRoutine(cancellationToken), cancellationToken);
        }

        private void PokerRoutine(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TimeSpan checkInterval = _checkInterval;
                    try
                    {
                        LinkedListNode<IPokableTask>? current;
                        lock (_lockObject)
                        {
                            // idle time exceeded; find last non-running
                            current = _tasks.Last;
                        }

                        TimeSpan timeFromLastExecution = _dateTimeService.UtcNow - _lastExecutionTime;

                        if (timeFromLastExecution > _idleTimeAllowed)
                        {
                            while (current != null)
                            {
                                if (!current.Value.IsRunning)
                                {
                                    _log.Debug("Poking");

                                    // this is the task that does not run the longest
                                    current.Value.Poke();
                                    checkInterval = _checkAfterPokeInterval; // wait a little bit longer after poking
                                    break;
                                }

                                current = current.Previous;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);

                        checkInterval = _errorInterval;
                    }

                    cancellationToken.WaitHandle.WaitOne(checkInterval);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
        }
    }
}
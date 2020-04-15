using System;
using System.Collections.Generic;
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
#warning TODO - unit tests

        private readonly IDateTimeService _dateTimeService;

        private readonly LinkedList<IPokableTask> _tasks = new LinkedList<IPokableTask>();
        private readonly object _lockObject = new object();

        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _checkAfterPokeInterval = TimeSpan.FromSeconds(2);
        private readonly TimeSpan _errorInterval = TimeSpan.FromSeconds(10);

        private System.DateTime _lastExecutionTime = System.DateTime.UtcNow;
        private readonly TimeSpan _idleTimeAllowed;

        public TaskMaster(IConfigurationReader configurationReader, IDateTimeService dateTimeService)
        {
            this._idleTimeAllowed = configurationReader.Configuration.Tasks.IdleTimeAllowed;
            this._dateTimeService = dateTimeService;
        }

        public void Register(IPokableTask pokableTask)
        {
            lock (_lockObject)
            {
                pokableTask.ExecutionFinished += PokableTaskOnExecutionFinished;
                _tasks.AddFirst(pokableTask);
            }
        }

        private void PokableTaskOnExecutionFinished(object sender, EventArgs e)
        {
#warning TEST
            try
            {
                if (sender is IPokableTask pokableTask)
                {
#warning CHECK - jakiś deadlock? - INFINITE LOCK!!! crap - trzeba może lepiej przeprojektować tą klasę ( w szczególności synchronizację i struktury danych)
                    lock (_lockObject)
                    {
                        if (!_tasks.Remove(pokableTask))
                        {
                            throw new InvalidOperationException("Task finished was not registered correctly");
                        }

                        _tasks.AddFirst(pokableTask);
                        _lastExecutionTime = _dateTimeService.UtcNow;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Fatal(ex);
            }
        }

        public void StartTasks(CancellationToken cancellationToken)
        {
            lock (_lockObject)
            {
                foreach (IPokableTask task in _tasks)
                {
                    task.Start(cancellationToken);
                }

                _lastExecutionTime = _dateTimeService.UtcNow;
            }

            Task.Run(() => PokerRoutine(cancellationToken), cancellationToken);
        }

        private void PokerRoutine(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    TimeSpan checkInterval = _checkInterval;
                    try
                    {
                        lock (_lockObject)
                        {
                            TimeSpan timeFromLastExecution = _dateTimeService.UtcNow - _lastExecutionTime;

                            if (timeFromLastExecution > _idleTimeAllowed)
                            {
                                // idle time exeeded; find last non-running
                                LinkedListNode<IPokableTask> current = _tasks.Last;

                                while (current != null)
                                {
                                    if (!current.Value.IsRunning)
                                    {
                                        // this is the task that does not run the longest
                                        current.Value.Poke();
                                        checkInterval = _checkAfterPokeInterval; // wait a little bit longer after poking
                                    }
                                    else
                                    {
                                        current = current.Previous;
                                    }
                                }
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
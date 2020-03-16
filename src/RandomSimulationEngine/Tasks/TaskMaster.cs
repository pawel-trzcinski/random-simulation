using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using RandomSimulationEngine.Configuration;

namespace RandomSimulationEngine.Tasks
{
    public class TaskMaster : ITaskMaster
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(TaskMaster));
#warning TODO
        private readonly LinkedList<IPokableTask> _tasks = new LinkedList<IPokableTask>();
        private readonly object _lockObject = new object();

        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _errorInterval = TimeSpan.FromSeconds(10);

        private readonly TimeSpan _idleTimeAllowed;

        public TaskMaster(IConfigurationReader configurationReader)
        {
            this._idleTimeAllowed = configurationReader.Configuration.Tasks.IdleTimeAllowed;
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
            try
            {
                if (sender is IPokableTask pokableTask)
                {
                    lock (_lockObject)
                    {
                        if (!_tasks.Remove(pokableTask))
                        {
                            throw new InvalidOperationException("Task finished ws not registered correctly");
                        }

                        _tasks.AddFirst(pokableTask);
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
            }

            Task.Run(()=>PokerRoutine(cancellationToken), cancellationToken);
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
                        // TaskMaster should register some methods or tokens that IPokableTask will be using to tell if it's running or not and what time did it finish last execution
                        // do last execution by means of LinkedList - finish executing - find task in the list and put it first. The last task in List (if not running) is the one
#warning TODO
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
using System;
using System.Threading;

namespace RandomSimulationEngine.Tasks
{
    public interface IPokableTask
    {
        event EventHandler ExecutionFinished;

        bool IsRunning { get; }

        void Start(CancellationToken cancellationToken);
        void Poke();
    }
}
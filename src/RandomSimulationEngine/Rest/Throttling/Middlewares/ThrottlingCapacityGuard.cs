using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RandomSimulationEngine.Rest.Throttling.Middlewares
{
    public class ThrottlingCapacityGuard
    {
        #region Fields

        private readonly SemaphoreSlim _guardSemaphore = new SemaphoreSlim(1, 1);

        private readonly IThrottlingOptions _options;
        private readonly LinkedList<TaskCompletionSource<EnqueueStatus>> _queue = new LinkedList<TaskCompletionSource<EnqueueStatus>>();

        private static readonly Task<EnqueueStatus> _queueFullTask = Task.FromResult(EnqueueStatus.QueueFull);
        private static readonly Task<EnqueueStatus> _queueCancelledTask = Task.FromResult(EnqueueStatus.Cancelled);

        private int _concurrentRequestsCount;

        #endregion Fields

        public ThrottlingCapacityGuard(IThrottlingOptions options)
        {
            this._options = options;
        }

        #region CheckCapacity and Enqueue

        public CapacityCheckResult CheckCapacity(CancellationToken requestAbortedCancellationToken)
        {
            try
            {
                CancellationToken enqueueCancellationToken = GetEnqueueCancellationToken(requestAbortedCancellationToken);

                _guardSemaphore.Wait(enqueueCancellationToken);

                if (_concurrentRequestsCount < _options.ConcurrentRequestsLimit)
                {
                    ++_concurrentRequestsCount;
                    return CapacityCheckResult.GetAllowed();
                }

                return CapacityCheckResult.GetQueued(Enqueue(enqueueCancellationToken));
            }
            catch (OperationCanceledException)
            {
                return CapacityCheckResult.GetCancelled();
            }
            finally
            {
                _guardSemaphore.Release();
            }
        }

        private Task<EnqueueStatus> Enqueue(CancellationToken enqueueCancellationToken)
        {
            Task<EnqueueStatus> enqueueTask = _queueFullTask;

            if (_options.QueueLimit > 0 && _queue.Count < _options.QueueLimit)
            {
                enqueueTask = InternalEnqueue(enqueueCancellationToken);
            }

            return enqueueTask;
        }

        private Task<EnqueueStatus> InternalEnqueue(CancellationToken enqueueCancellationToken)
        {
            Task<EnqueueStatus> enqueueTask = _queueCancelledTask;

            if (enqueueCancellationToken.IsCancellationRequested)
            {
                return enqueueTask;
            }

            var enqueueTaskCompletionSource = new TaskCompletionSource<EnqueueStatus>(TaskCreationOptions.RunContinuationsAsynchronously);
            enqueueCancellationToken.Register(CancelEnqueue, enqueueTaskCompletionSource);

            _queue.AddLast(enqueueTaskCompletionSource);
            enqueueTask = enqueueTaskCompletionSource.Task;

            return enqueueTask;
        }

        private CancellationToken GetEnqueueCancellationToken(CancellationToken requestAbortedCancellationToken)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(requestAbortedCancellationToken, GetTimeoutToken()).Token;
        }

        private CancellationToken GetTimeoutToken()
        {
            var timeoutTokenSource = new CancellationTokenSource();

            CancellationToken timeoutToken = timeoutTokenSource.Token;

            timeoutTokenSource.CancelAfter(_options.QueueTimeout);

            return timeoutToken;
        }

        #endregion CheckCapacity and Enqueue

        #region dequeue

        public void FinishExecution()
        {
            try
            {
                _guardSemaphore.Wait();

                // attempt to dequeue a task
                if (_queue.Count > 0)
                {
                    InternalDequeue();
                }
                else
                {
                    // if task not dequeued, then let's release 'worker' for another request
                    --_concurrentRequestsCount;
                }
            }
            finally

            {
                _guardSemaphore.Release();
            }
        }

        private void InternalDequeue()
        {
            TaskCompletionSource<EnqueueStatus> enqueueTaskCompletionSource = _queue.First.Value;

            _queue.RemoveFirst();

            enqueueTaskCompletionSource.SetResult(EnqueueStatus.AllowExecution);
        }

        #endregion dequeue

        private void CancelEnqueue(object state)
        {
            try
            {
                _guardSemaphore.Wait();
                var enqueueTaskCompletionSource = (TaskCompletionSource<EnqueueStatus>) state;

                if (_queue.Remove(enqueueTaskCompletionSource))
                {
                    enqueueTaskCompletionSource.SetResult(EnqueueStatus.Cancelled);
                }
            }
            finally

            {
                _guardSemaphore.Release();
            }
        }
    }
}
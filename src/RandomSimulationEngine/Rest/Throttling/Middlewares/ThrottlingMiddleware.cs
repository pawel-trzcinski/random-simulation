using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using SeaChange.Prodis.ProdisCommon.Rest.Throttling;

namespace RandomSimulationEngine.Rest.Throttling.Middlewares
{
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ThrottlingCapacityGuard _capacityGuard;

        public ThrottlingMiddleware(RequestDelegate next, IThrottlingOptions options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _capacityGuard = new ThrottlingCapacityGuard(options);
        }

        public async Task Invoke(HttpContext context)
        {
            CapacityCheckResult checkResult = _capacityGuard.CheckCapacity(context.RequestAborted);

            if (!checkResult.ExecutionAllowed)
            {
                EnqueueStatus enqueueStatus = await checkResult.QueueTask; // this is the place task waits in queue

                // execution cancelled or queue timeout reached or queue full
                if (enqueueStatus != EnqueueStatus.AllowExecution && !context.RequestAborted.IsCancellationRequested)
                {
                    var responseFeature = context.Features.Get<IHttpResponseFeature>();
                    responseFeature.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    responseFeature.ReasonPhrase = enqueueStatus.ToString();

                    return;
                }
            }

            try
            {
                await _next(context);
            }
            finally
            {
                _capacityGuard.FinishExecution();
            }

        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc;
using RandomSimulationEngine.Rest;

namespace RandomSimulation.Tests.Tasks.ImageDownload
{
    public class TestController : Controller
    {
        public const string ACTION_VERB = "unittest";

        [HttpGet(ACTION_VERB)]
        public ContentResult Test()
        {
            return Content(Guid.NewGuid().ToString(), RandomSimulationController.TEXT_PLAIN);
        }
    }
}
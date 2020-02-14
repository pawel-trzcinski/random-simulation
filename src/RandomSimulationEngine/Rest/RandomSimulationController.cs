using RandomSimulationEngine.Rest.HeaderAttributes;
using log4net;
using Microsoft.AspNetCore.Mvc;
using RandomSimulationEngine.ValueCalculator;
using System.Globalization;
using System;
using RandomSimulationEngine.RandomBytesPuller;
using System.ComponentModel.DataAnnotations;

namespace RandomSimulationEngine.Rest
{
    /// <summary>
    /// Default ClassNamer controller.
    /// </summary>
    public class RandomSimulationController : Controller, IRandomSimulationController
    {
#warning TODO - unit tests
        /// <summary>
        /// Accept header name.
        /// </summary>
        public const string ACCEPT = "Accept";

        /// <summary>
        /// text/plain content type.
        /// </summary>
        public const string TEXT_PLAIN = "text/plain";

        private static readonly string robotsContent = System.IO.File.ReadAllText("robots.txt");

        private readonly IValueCalculator valueCalculator;
        private readonly IRandomBytesPuller randomBytesPuller;

        public RandomSimulationController(IValueCalculator valueCalculator, IRandomBytesPuller randomBytesPuller)
        {
            this.valueCalculator = valueCalculator;
            this.randomBytesPuller = randomBytesPuller;
        }

        /// <summary>
        /// Method used for debugging as well as for health-check actions.
        /// </summary>
        /// <returns>Always 200.</returns>
        [HttpGet("test")]
        public StatusCodeResult Test()
        {
            return this.Ok();
        }

        [HttpGet("next")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Next()
        {
            return Content(this.valueCalculator.GetInt32(this.randomBytesPuller.Pull(4)).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next/{min}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Next([FromRoute] int min)
        {
            return Content(this.valueCalculator.GetInt32(this.randomBytesPuller.Pull(4), min).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next/{min}/{max}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Next([FromRoute]int min, [FromRoute]int max)
        {
            return Content(this.valueCalculator.GetInt32(this.randomBytesPuller.Pull(4), min, max).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next-double")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult NextDouble()
        {
            return Content(this.valueCalculator.GetDouble(this.randomBytesPuller.Pull(8)).ToString("G17", CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next-bytes/{count}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult NextBytes([FromRoute][Range(1, 50)]int count)
        {
#warning TODO - check czy ten Range tutaj działa
            return Content(Convert.ToBase64String(this.randomBytesPuller.Pull(count)), TEXT_PLAIN);
        }

        [HttpGet("health")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Health()
        {
#warning TODO - sources health check - ale tylko generalnie, żeby nie zdradzać jakie ani nawet ile; tylko np FULL, PRAWIE FULL, NIEZABARDZO, PRAWIE TRUP, TRUP
            return null;
        }

        /// <summary>
        /// Standard robots file.
        /// </summary>
        /// <returns>Robots file content.</returns>
        [HttpGet("robots.txt")]
        public ContentResult Robots()
        {
            return Content(robotsContent, TEXT_PLAIN);
        }
    }
}
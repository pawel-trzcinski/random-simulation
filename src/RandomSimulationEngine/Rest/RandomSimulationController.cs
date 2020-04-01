using RandomSimulationEngine.Rest.HeaderAttributes;
using Microsoft.AspNetCore.Mvc;
using RandomSimulationEngine.ValueCalculator;
using System.Globalization;
using System;
using RandomSimulationEngine.Health;
using RandomSimulationEngine.RandomBytesPuller;

namespace RandomSimulationEngine.Rest
{
    /// <summary>
    /// Default RandomSimulation controller.
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

        private static readonly string _robotsContent = System.IO.File.ReadAllText("robots.txt");

        private readonly IValueCalculator _valueCalculator;
        private readonly IRandomBytesPuller _randomBytesPuller;
        private readonly IHealthChecker _healthChecker;

        public RandomSimulationController(IValueCalculator valueCalculator, IRandomBytesPuller randomBytesPuller, IHealthChecker healthChecker)
        {
            this._valueCalculator = valueCalculator;
            this._randomBytesPuller = randomBytesPuller;
            this._healthChecker = healthChecker;
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
            return Content(this._valueCalculator.GetInt32(this._randomBytesPuller.Pull(4)).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next/{max}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Next([FromRoute] int max)
        {
            return Content(this._valueCalculator.GetInt32(this._randomBytesPuller.Pull(4), max).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next/{min}/{max}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult Next([FromRoute]int min, [FromRoute]int max)
        {
            return Content(this._valueCalculator.GetInt32(this._randomBytesPuller.Pull(4), min, max).ToString(CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next-double")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult NextDouble()
        {
            return Content(this._valueCalculator.GetDouble(this._randomBytesPuller.Pull(8)).ToString("G17", CultureInfo.InvariantCulture), TEXT_PLAIN);
        }

        [HttpGet("next-bytes/{count:range(1,50)}")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public ContentResult NextBytes([FromRoute]int count)
        {
            return Content(Convert.ToBase64String(this._randomBytesPuller.Pull(count)), TEXT_PLAIN);
        }

        [HttpGet("health")]
        [AddCorsHeader]
        [AddGitHubHeader]
        public IActionResult Health()
        {
            HealthStatus healthStatus = _healthChecker.GetHealthStatus();

            if (healthStatus == HealthStatus.Dead)
            {
                return StatusCode(500);
            }

            return Content(((int) healthStatus).ToString());
        }

        /// <summary>
        /// Standard robots file.
        /// </summary>
        /// <returns>Robots file content.</returns>
        [HttpGet("robots.txt")]
        public ContentResult Robots()
        {
            return Content(_robotsContent, TEXT_PLAIN);
        }
    }
}
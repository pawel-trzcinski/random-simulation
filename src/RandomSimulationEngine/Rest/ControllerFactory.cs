using System;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace RandomSimulationEngine.Rest
{
    /// <summary>
    /// Factory for creating scoped controller with use of <see cref="SimpleInjector"/>.
    /// </summary>
    public class ControllerFactory : IControllerFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ControllerFactory));

        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerFactory"/> class.
        /// </summary>
        /// <param name="container"><see cref="SimpleInjector"/> container.</param>
        public ControllerFactory(Container container)
        {
            this._container = container;
        }

        /// <inheritdoc/>
        public object CreateController(ControllerContext context)
        {
            _log.Debug("Creating controller");

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Scope scope = ThreadScopedLifestyle.BeginScope(_container);

            _log.Debug("Seting Scope feature");
            context.HttpContext.Features.Set(scope);

            _log.Debug("Getting controller from incection container");
            return scope.GetInstance<IRandomSimulationController>();
        }

        /// <inheritdoc/>
        public void ReleaseController(ControllerContext context, object controller)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _log.Debug("Disposing of Scope feature");
            context.HttpContext.Features.Get<Scope>().Dispose();
        }
    }
}
using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RandomSimulationEngine.Rest.HeaderAttributes
{
    /// <summary>
    /// Add any custom header to response.
    /// </summary>
    public abstract class AddHeaderAttribute : ResultFilterAttribute
    {
        private readonly string name;
        private readonly string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeaderAttribute"/> class.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        protected AddHeaderAttribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <inheritdoc/>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.HttpContext.Response.Headers.Add(name, new string[] { value });
            base.OnResultExecuting(context);
        }
    }
}

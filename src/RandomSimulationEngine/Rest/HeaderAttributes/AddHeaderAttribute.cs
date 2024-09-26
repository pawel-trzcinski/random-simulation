using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RandomSimulationEngine.Rest.HeaderAttributes
{
    /// <summary>
    /// Add any custom header to response.
    /// </summary>
    public abstract class AddHeaderAttribute : ResultFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddHeaderAttribute"/> class.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        protected AddHeaderAttribute(string name, string value)
        {
            this._name = name;
            this._value = value;
        }

        /// <inheritdoc/>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.HttpContext.Response.Headers.Append(_name, new[] {_value});
            base.OnResultExecuting(context);
        }
    }
}
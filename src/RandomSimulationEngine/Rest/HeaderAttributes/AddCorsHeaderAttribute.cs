namespace RandomSimulationEngine.Rest.HeaderAttributes
{
    /// <summary>
    /// Attribute for adding CORS header.
    /// </summary>
    public class AddCorsHeaderAttribute : AddHeaderAttribute
    {
        /// <summary>
        /// Name of CORS header.
        /// </summary>
        public const string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCorsHeaderAttribute"/> class.
        /// </summary>
        public AddCorsHeaderAttribute()
            : base(ACCESS_CONTROL_ALLOW_ORIGIN, "*")
        {
        }
    }
}
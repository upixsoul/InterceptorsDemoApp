namespace InterceptorDemoApp.Api.Filters
{
    /// <summary>
    /// Marks a service method for interception by <see cref="Interceptors.TestServiceInterceptor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TestInterceptorAttribute : Attribute
    {
    }
}

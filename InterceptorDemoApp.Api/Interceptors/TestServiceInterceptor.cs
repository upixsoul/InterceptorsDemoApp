using Castle.DynamicProxy;
using InterceptorDemoApp.Api.Filters;

namespace InterceptorDemoApp.Api.Interceptors
{
    /// <summary>
    /// Castle DynamicProxy interceptor that runs before/after service methods
    /// decorated with <see cref="TestInterceptorAttribute"/>.
    /// </summary>
    public sealed class TestServiceInterceptor : IInterceptor
    {
        private readonly ILogger<TestServiceInterceptor> logger;

        public TestServiceInterceptor(ILogger<TestServiceInterceptor> logger)
        {
            this.logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var hasAttribute = method.GetCustomAttributes(typeof(TestInterceptorAttribute), inherit: true).Length > 0;

            if (!hasAttribute)
            {
                invocation.Proceed();
                return;
            }

            var methodName = method.Name;
            logger.LogInformation("[TestInterceptor] Before executing method: {MethodName}", methodName);

            invocation.Proceed();

            logger.LogInformation("[TestInterceptor] After executing method: {MethodName}", methodName);
        }
    }
}

using Castle.DynamicProxy;
using InterceptorDemoApp.Api.Interceptors;
using InterceptorDemoApp.Api.Filters;
using InterceptorDemoApp.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InterceptorDemoApp.Api.Extensions
{
    public static class InterceptedServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a concrete service and exposes it through its interface
        /// using a Castle DynamicProxy that applies <see cref="TestServiceInterceptor"/>.
        /// </summary>
        public static IServiceCollection AddInterceptedScoped<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddScoped<TImplementation>();
            services.AddScoped<TInterface>(provider =>
            {
                var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
                var implementation = provider.GetRequiredService<TImplementation>();
                var interceptor = provider.GetRequiredService<TestServiceInterceptor>();

                return (TInterface)proxyGenerator.CreateInterfaceProxyWithTarget(
                    typeof(TInterface),
                    implementation,
                    interceptor);
            });

            return services;
        }

        /// <summary>
        /// Registers all services related to interception in one call.
        /// - ProxyGenerator
        /// - TestServiceInterceptor
        /// - GlobalExecutionTimerFilter
        /// - Example intercepted IOrderService -> OrderService
        /// </summary>
        public static IServiceCollection AddInterceptors(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                // Register the global execution timer filter (interceptor) for all controller actions
                options.Filters.Add<GlobalExecutionTimerFilter>();
            });

            services.AddSingleton<ProxyGenerator>();
            services.AddScoped<TestServiceInterceptor>();
            services.AddScoped<GlobalExecutionTimerFilter>();

            // Register the application's intercepted domain services here.
            services.AddInterceptedScoped<IOrderService, OrderService>();

            return services;
        }
    }
}

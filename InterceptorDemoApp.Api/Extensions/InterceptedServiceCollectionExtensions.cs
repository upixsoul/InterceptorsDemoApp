using InterceptorDemoApp.Api.Filters;
using InterceptorDemoApp.Api.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InterceptorDemoApp.Api.Extensions
{
    public static class InterceptedServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a service with a decorator in the DI container.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <typeparam name="TDecorator"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDecoratedScoped<TInterface, TImplementation, TDecorator>(
        this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
        where TDecorator : class, TInterface
        {
            services.AddScoped<TImplementation>();
            services.AddScoped<TInterface>(sp =>
            {
                var inner = (TInterface)sp.GetRequiredService<TImplementation>();
                return ActivatorUtilities.CreateInstance<TDecorator>(sp, inner);
            });

            return services;
        }

        /// <summary>
        /// Registers the global execution timer filter and the IOrderService with its logging decorator in the DI container.
        /// - GlobalExecutionTimerFilter is applied to all controller actions.
        /// - IOrderService is intercepted with a logging decorator.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInterceptors(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                // Register the global execution timer filter (interceptor) for all controller actions
                options.Filters.Add<GlobalExecutionTimerFilter>();
            });

            services.AddScoped<GlobalExecutionTimerFilter>();

            // Register the IOrderService with its implementation and apply the logging decorator
            services.AddScoped<IOrderService, OrderService>();
            services.Decorate<IOrderService, OrderServiceLoggingDecorator>();

            return services;
        }
    }
}

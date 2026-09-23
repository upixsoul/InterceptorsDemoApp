namespace InterceptorDemoApp.Api.Services
{
    public sealed class OrderServiceLoggingDecorator : IOrderService
    {
        private readonly IOrderService inner;
        private readonly ILogger<OrderServiceLoggingDecorator> logger;

        public OrderServiceLoggingDecorator(
            IOrderService inner,
            ILogger<OrderServiceLoggingDecorator> logger)
        {
            this.inner = inner;
            this.logger = logger;
        }

        public void SomeLogic()
        {
            logger.LogInformation("[Decorator] Before executing method: {MethodName}", nameof(SomeLogic));
            inner.SomeLogic();
            logger.LogInformation("[Decorator] After executing method: {MethodName}", nameof(SomeLogic));
        }
    }
}

using Microsoft.Extensions.Logging;

namespace InterceptorDemoApp.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> logger;

        public OrderService(ILogger<OrderService> logger)
        {
            this.logger = logger;
        }

        public void SomeLogic()
        {
            logger.LogInformation("SomeLogic method executed successfully.");
        }
    }
}

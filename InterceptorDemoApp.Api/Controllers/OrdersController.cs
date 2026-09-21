using InterceptorDemoApp.Api.Filters;
using InterceptorDemoApp.Api.Models;
using InterceptorDemoApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterceptorDemoApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuditLogging] // Scoped interceptor applied to all actions in this controller
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> logger;
        private readonly InterceptorDemoApp.Api.Services.IOrderService orderService;

        private static readonly List<object> Orders = new()
    {
        new { OrderId = 101, Item = "Mechanical Keyboard", Price = 129.99 },
        new { OrderId = 102, Item = "UltraWide Monitor", Price = 499.50 }
    };

        public OrdersController(
            ILogger<OrdersController> logger, 
            IOrderService orderService) =>
            (this.logger, this.orderService) = (logger, orderService);

        /// <summary>
        /// Retrieves all orders.
        /// Triggers both ExecutionTimeGlobalFilter and AuditLoggingAttribute.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            logger.LogInformation("Retrieving all orders.");
            orderService.SomeLogic();
            return Ok(new { OrdersCount = Orders.Count, Items = Orders });
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            logger.LogInformation("Creating a new order for item: {ItemName}", request.ItemName);
            var newOrder = new { 
                OrderId = new Random().Next(103, 999), 
                Item = request.ItemName, Price = 99.00 
            };

            Orders.Add(newOrder);
            return CreatedAtAction(nameof(GetAll), newOrder);
        }
    }
}

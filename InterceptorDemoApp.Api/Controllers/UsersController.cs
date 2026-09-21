using Microsoft.AspNetCore.Mvc;

namespace InterceptorDemoApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(
        ILogger<UsersController> logger
        ) : ControllerBase
    {
        private static readonly List<string> Users = new() { "Alice", "Bob", "Charlie" };

        /// <summary>
        /// Retrieves all users.
        /// Subject only to the Global ExecutionTimeGlobalFilter.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            logger.LogInformation("Retrieving all users.");
            return Ok(new { Total = Users.Count, Data = Users });
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            logger.LogInformation($"Retrieving user with ID: {id}");
            if (id < 0 || id >= Users.Count)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new { Id = id, Name = Users[id] });
        }
    }
}

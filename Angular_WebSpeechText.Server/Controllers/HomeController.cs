using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Angular_WebSpeechText.Models;


namespace Angular_WebSpeechText.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserContext _context;

        public HomeController(ILogger<HomeController> logger, UserContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok(new { Message = "Welcome to the Home page!" });
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            var message = "Privacy policy page placeholder.";
            return Ok(new { Message = message });
        }

        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {           
            var errorInfo = new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = "An error occurred" };
            return StatusCode(500, errorInfo);
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // на сервере не нужно очищать сессии, так как используются JWT
            return Ok(new { Message = "User has been logged out" });
        }
    }
}

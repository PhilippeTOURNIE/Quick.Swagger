using Microsoft.AspNetCore.Mvc;

namespace Quick.SwaggerSample.Controllers.V2
{
    [ApiVersion("2.0", Deprecated = false)]
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {


        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [MapToApiVersion("2.0")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World ++");
        }
    }
}
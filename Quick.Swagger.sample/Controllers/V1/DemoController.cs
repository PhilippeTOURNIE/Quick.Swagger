using Microsoft.AspNetCore.Mvc;

namespace Quick.SwaggerSample.Controllers.V1
{
    [ApiVersion("1.0", Deprecated = false)]
    [ApiVersion("1.1", Deprecated = false)]
    [ApiController]
    [Route("/api/v{version:apiVersion}/[controller]")]
    public class DemoController : ControllerBase
    {


        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World 1.0");
        }

        [MapToApiVersion("1.1")]
        [HttpGet]
        public IActionResult Get1()
        {
            return Ok("Hello World 1.1");
        }
    }
}
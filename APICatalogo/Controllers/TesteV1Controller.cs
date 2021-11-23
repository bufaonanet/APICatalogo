using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("1.0")]   
    [Route("api/teste")]
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("<html><body><h2>TesteV1Controller - GET V 1.0 </h2></body></html>", "text/html");
        }       
    }
}
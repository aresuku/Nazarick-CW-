using Microsoft.AspNetCore.Mvc;
namespace HairSalon.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult GetConfig()
        {
            var appSettings = new
            {
                AppName = _configuration["AppSettings:AppName"],
                Version = _configuration["AppSettings:Version"],
                MaxItems = _configuration.GetValue<int>("AppSettings:MaxItems")
            };
            return Ok(appSettings);
        }
    }
}
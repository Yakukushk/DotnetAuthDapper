using DotnetAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DataContextDapper _dataContextDapper;
        private readonly IConfiguration _configuration;
        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
            _dataContextDapper = new DataContextDapper(_configuration);
        }

        [HttpGet("TestConnection")]
      public DateTime TestConnection()
        {
            return _dataContextDapper.LoadSingleData<DateTime>("Select GetDate()");
        }
    }
}

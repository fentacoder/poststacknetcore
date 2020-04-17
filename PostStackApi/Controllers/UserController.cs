using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PostStackDataAccessLibrary.DbModels;

namespace PostStackApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Produces("text/html")]
        public string Get()
        {
            string htmlString = @"
            <h1>Api home page</h1>
            ";

            return htmlString;
        }

        [HttpPost]
        [Route("register")]
        public ActionResult<int> Register([FromBody] string jsonObj)
        {
            User values = JsonSerializer.Deserialize<User>(jsonObj);

            var user = new User();

            int userId = user.RegisterUser(_configuration.GetConnectionString("DefaultConnection"),
                values.Name, values.Email, values.Password);

            return userId;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<int> Login([FromBody] string jsonObj)
        {
            User values = JsonSerializer.Deserialize<User>(jsonObj);

            var user = new User();

            int userId = user.LogUserIn(_configuration.GetConnectionString("DefaultConnection"),
                values.Email, values.Password);

            return userId;
        }
    }
}
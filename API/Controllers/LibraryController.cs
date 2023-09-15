using API.DataAccess;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
         public LibraryController(IDataAccess dataAccess, IConfiguration configuration = null) 
         {
            this._dataAccess = dataAccess;
            this._configuration = configuration;
         }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount(User user)
        {
            if(!_dataAccess.IsEmailAvailable(user.Email))
            {
                return Ok("Email is not available");
            }
            user.CreatedOn = DateTime.Now.ToString();
            user.UserType = UserType.USER;
            _dataAccess.CreateUser(user);
            return Ok("Account created successfully!");
        }

        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
           if(_dataAccess.AuthenticateUser(email, password, out User? user))
           {
                if(user != null)
                {
                    var jwt = new Jwt(_configuration["Jwt:Key"], _configuration["Jwt:Duration"]);
                    var token = jwt.GenerateToken(user);
                    return Ok(token);
                }
           }
            return Ok("Invalid");
        }
    }
}

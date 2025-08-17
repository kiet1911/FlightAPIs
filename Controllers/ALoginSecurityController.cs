using FlightAPIs.Controllers.RequestData;
using FlightAPIs.Helper;
using FlightAPIs.Internal.Token;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ALoginSecurityController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;//to take data from appsetting 
        public ILogger<UserController> Logger;
        public ALoginSecurityController(IConfiguration _configuration, ILogger<UserController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //Login admin or employee , Create token and LogIn
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(String Name, String Password)
        {
            var userDetail = await db_contex.Admins.Where(u => u.Email == Name.Trim() && u.Password == Password.Trim()).FirstOrDefaultAsync();
            if (userDetail == null)
            {
                return BadRequest();
            }
            EmployeeTokenProvider tokenPro = new EmployeeTokenProvider(Configuration);
            var token = tokenPro.createEmployeeToken(userDetail);
            return Ok(token);
        }
    }
}

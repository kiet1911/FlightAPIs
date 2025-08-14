using FlightAPIs.Controllers.RequestData;
using FlightAPIs.Internal.Token;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FlightAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;
        public UserController(IConfiguration configuration) {
            this.Configuration = configuration;
        }

        //Login , Create token
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(String userName, String userPassWord)
        {
            var userDetail = await db_contex.Users.Where(u => u.Email == userName.Trim() && u.Password == userPassWord.Trim()).FirstOrDefaultAsync();
            if (userDetail == null) {
                return BadRequest();
            }
            TokenProvider tokenPro = new TokenProvider(Configuration);
            var token = tokenPro.CreateTokenWithUser(userDetail);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<List<User>?> GetUser()
        {
            try {
                var Users = await db_contex.Users.ToListAsync();
                if (Users == null || !Users.Any())
                {
                    return null;
                }
                return Users;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> getUserId([FromQuery] GetUserId i)
        {
            try
            {
                var user = await db_contex.Users.FindAsync(i.Id);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized();

            }
        }
    }
}

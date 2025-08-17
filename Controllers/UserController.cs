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
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;//to take data from appsetting 
        public ILogger<UserController> Logger;
        public UserController(IConfiguration _configuration,ILogger<UserController> _logger) {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        ////Login admin or employee , Create token and LogIn
        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> Login(String Name, String Password)
        //{
        //    var userDetail = await db_contex.Admins.Where(u => u.Email == Name.Trim() && u.Password == Password.Trim()).FirstOrDefaultAsync();
        //    if (userDetail == null) {
        //        return BadRequest();
        //    }
        //    EmployeeTokenProvider tokenPro = new EmployeeTokenProvider(Configuration);
        //    var token = tokenPro.createEmployeeToken(userDetail);
        //    return Ok(token);
        //}
        //create user
        [Authorize(Policy = "roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> userCreate([FromForm] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            try { 
            bool userEmailInvalid = db_contex.Users.Where(u => u.Email == user.Email).Any();
            if (userEmailInvalid)
            {
                return BadRequest("the email has already been used");
            }

            //check user other information 
            UserCheckInfor errorInfor = new UserCheckInfor();
            Dictionary<string,string> errorDictionary = errorInfor.userCheckInformation(user);
                errorDictionary.TryGetValue("Error", out string? errorText);
                if(errorText != null)
                {
                    return BadRequest(errorText);
                }
            //type user always 1 
            user.UserType = 1;
            await db_contex.Users.AddAsync(user);
            await db_contex.SaveChangesAsync();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            return Ok("new user has been added successful");
            
        }
        //update user
        [Authorize(Policy = "roleSecurity")]
        [HttpPut]
        public async Task<IActionResult> userUpdate([FromForm] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            try
            {
                bool userExist = db_contex.Users.Where(u => u.Id == user.Id).Any();
                bool userEmailInvalid = db_contex.Users.Where(u => u.Id != user.Id && u.Email == user.Email ).Any();

                if(!userExist)
                {
                    return BadRequest("invalid user");
                }
                if (userEmailInvalid == true)
                {
                    return BadRequest("the email has already been used");
                }

                //check user other information 
                UserCheckInfor errorInfor = new UserCheckInfor();
                Dictionary<string, string> errorDictionary = errorInfor.userCheckInformation(user);
                errorDictionary.TryGetValue("Error", out string? errorText);
                if (errorText != null)
                {
                    return BadRequest(errorText);
                }
                //type user always 1 
                user.UserType = 1;
                db_contex.Entry(user).State = EntityState.Modified;
                await db_contex.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("user has been updated successful");

        }
        //delete user by id
        [Authorize(Policy = "roleSecurity")]
        [HttpDelete]
        public IActionResult user([FromQuery]int id)
        {
            User? user = db_contex.Users.Where(u => u.Id == id).FirstOrDefault();
            try
            {
                if (user != null)
                {
                    db_contex.Users.Remove(user);
                    db_contex.SaveChanges();
                }
                else
                {
                    return BadRequest(String.Format("invalid user with id:{0}",id)  );
                }
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
            return Ok("delete success user with id:" + String.Format("{0}", id));
        }
        //take signle user by id
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> userDetailById([FromQuery] GetUserId i)
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
        //take user by pagition 
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> userOffset(int? id = 0)
        {
            int skipAmount = (int)id * 3;
            List<User> userLists;
            try
            {
                userLists = await db_contex.Users.OrderBy(p => p.Id).Skip(skipAmount).Take(3).ToListAsync();
                if (userLists == null)
                {
                    return Ok("user is over");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException?.Message);
            }
            return Ok(userLists);
        }
        //Take all user  
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<List<User>?> userAll()
        {
            Logger.LogInformation("start GetUser At {0}", DateTime.Now.Millisecond);//start logging 
            try
            {
                var Users = await db_contex.Users.ToListAsync();
                if (Users == null || !Users.Any())
                {
                    Logger.LogWarning("Users table is empty");//logging warning non Users
                    return null;
                }
                Logger.LogInformation("end GetUser At {0}", DateTime.Now.Millisecond);//end logging
                return Users;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);//logging error 
                return null;
            }
        }

    }
}

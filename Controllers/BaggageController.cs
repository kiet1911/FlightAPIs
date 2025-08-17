using FlightAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaggageController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;//to take data from appsetting 
        public ILogger<UserController> Logger;
        public BaggageController(IConfiguration _configuration, ILogger<UserController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //create 
        [Authorize(Policy = "roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> baggageCreate([FromForm] Baggage baggage)
        {
            baggage.User = null;
            baggage.CarryonBag = 7;
            if (baggage.SignedLuggage < 0 || baggage.SignedLuggage > 15)
            {
                return BadRequest("the signedLuggage should not higher than 15 and below to 0");
            }
            //exists code in ticket 
            //copare onle one code unique in baggage
            bool ticketExists = db_contex.TicketManagers.Where(u => u.Code == baggage.Code).Any();
            if (!ticketExists)
            {
                return BadRequest("the ticket is not exists please check again");
            }
            bool baggageExists = db_contex.Baggages.Where(u => u.Code == baggage.Code).Any();
            if (baggageExists)
            {
                return BadRequest("the code is exists please check again");
            }
            //exists user_id 
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    db_contex.Baggages.Add(baggage);
                    db_contex.SaveChanges();
                    transaction.Commit();
                    return Ok("create success baggage");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        transaction.Rollback();
                        return BadRequest(sqlEx.Message);
                    }
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.InnerException!.Message);
                }
            }
        }
        //edit
        [Authorize(Policy = "roleSecurity")]
        [HttpPut]
        public async Task<IActionResult> baggageUpdate([FromForm] Baggage baggage)
        {
            baggage.User = null;
            baggage.CarryonBag = 7;
            if (baggage.SignedLuggage < 0 || baggage.SignedLuggage > 15)
            {
                return BadRequest("the signedLuggage should not higher than 15 and below to 0");
            }
            //exists code in ticket 
            //copare onle one code unique in baggage
            bool baggageExistsId = db_contex.Baggages.Where(u => u.Code == baggage.Code && u.Id == baggage.Id).Any();
            if (!baggageExistsId)
            {
                return BadRequest("the baggage is not exists please check again");
            }
            bool baggageExists = db_contex.Baggages.Where(u => u.Code == baggage.Code && u.Id != baggage.Id).Any();
            if (baggageExists)
            {
                return BadRequest("the code is exists please check again");
            }
            //exists user_id 
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    db_contex.Baggages.Update(baggage);
                    db_contex.SaveChanges();
                    transaction.Commit();
                    return Ok("update success baggage");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        transaction.Rollback();
                        return BadRequest(sqlEx.Message);
                    }
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.InnerException!.Message);
                }
            }
        }
        //delete
        [Authorize(Policy = "roleSecurity")]
        [HttpDelete]
        public async Task<IActionResult> baggageDelete(int id)
        {
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    db_contex.Baggages.Remove(await db_contex.Baggages.FindAsync(id));
                    db_contex.SaveChanges();
                    transaction.Commit();
                    return Ok("delete baggage success");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        transaction.Rollback();
                        return BadRequest(sqlEx.Message);
                    }
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }
        //get 
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> baggageAll()
        {
            try
            {
                return Ok(await db_contex.Baggages.ToListAsync());
            }
            catch(SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> baggageById(int id)
        {
            try
            {
                return Ok(await db_contex.Baggages.FindAsync(id));
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> baggageUserId(int userId)
        {
            try
            {
                return Ok(await db_contex.Baggages.Where(u=>u.UserId == userId).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> baggageCode(String CodeDetail)
        {
            try
            {
                return Ok(await db_contex.Baggages.Where(u => u.Code.Contains(CodeDetail)).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

    }
}

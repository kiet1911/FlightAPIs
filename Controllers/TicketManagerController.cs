using FlightAPIs.Helper;
using FlightAPIs.Internal.DTOdata;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TicketManagerController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_context = new DbAbb296Kuphe1980Context();//connection
        public IConfiguration Configuration;//to config data from appsetting 
        public ILogger<FlightScheduleController> Logger;//logging,messager to cli 
        public TicketManagerController(IConfiguration _configuration, ILogger<FlightScheduleController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //create
        [Authorize(Policy = "roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> ticketCreate([FromForm]TicketDTO ticketDTO)
        {
            Dictionary<string,string> ticketError = await TicketValidation.ticketValidation(ticketDTO, db_context,false);
            ticketError.TryGetValue("Error", out string errerMessage);
            if(errerMessage != null)
            {
                return BadRequest(errerMessage);
            }
            //take seat using transaction  
            Seat seat = db_context.Seats.Where(u => u.FlightSchedulesId == ticketDTO.FlightSchedulesId && u.Seat1 == ticketDTO.SeatLocation.ToString()).FirstOrDefault();
            seat.FlightSchedules = null;
            using(var transaction = db_context.Database.BeginTransaction())
            {
                try
                {   //update seat
                    seat.Isbooked = 1;
                    db_context.Seats.Update(seat);
                    //add ticket 
                    TicketManager ticket = new TicketManager
                    {
                        FlightSchedulesId = ticketDTO.FlightSchedulesId,
                        UserId = ticketDTO.UserId,
                        Status = 0,
                        Code = ticketDTO.Code,
                        SeatLocation = ticketDTO.SeatLocation,
                        PayId = ticketDTO.PayId
                    };
                    //update flight
                    FlightSchedule flightSchedule = await db_context.FlightSchedules.Where(u => u.Id == ticketDTO.FlightSchedulesId).FirstOrDefaultAsync();
                    flightSchedule.BookedSeats += 1;

                    db_context.TicketManagers.Add(ticket);
                    db_context.FlightSchedules.Update(flightSchedule);
                    db_context.SaveChanges();
                    transaction.Commit();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);

                }
                catch(SqlException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return Ok("add success ticket");
        }
        //edit
        [Authorize(Policy = "roleSecurity")]
        [HttpPut]
        public async Task<IActionResult> ticketUpdate([FromForm] TicketDTO ticketDTO)
        {
            //only update userid , status , payid
            //validation
            Dictionary<string, string> ticketError = await TicketValidation.ticketValidation(ticketDTO, db_context,true);
            ticketError.TryGetValue("Error", out string errerMessage);
            if (errerMessage != null)
            {
                return BadRequest(errerMessage);
            }
            //take ticket before 
            TicketManager ticketManager = db_context.TicketManagers.Where(u => u.Id == ticketDTO.Id).FirstOrDefault();
            if(ticketManager == null)
            {
                return BadRequest("ticketId is invalid");
            }
            using (var transaction = db_context.Database.BeginTransaction())
            {
                try
                {
                    //update 
                    ticketManager.UserId = ticketDTO.UserId;
                    ticketManager.Status = ticketDTO.Status;
                    ticketManager.PayId = ticketDTO.PayId;
                    db_context.TicketManagers.Update(ticketManager);
                    db_context.SaveChanges();
                    transaction.Commit();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);

                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return Ok("update success ticket");

        }
        //delete no delete

        //read : all , byID , userId Status , seat , 
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> ticketALL()
        {
            return Ok(await db_context.TicketManagers.ToListAsync());
        }

        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> ticketUserIdStatus(int UserId , int status)
        {
            try
            {
                return Ok(await db_context.TicketManagers.Where(u=>u.UserId == UserId && u.Status == status).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> ticketByObject(string type, int data)
        {
            var arrayAttribute = new string[] { "Id", "FlightSchedulesId", "UserId", "Status", "SeatLocation", "PayId" };
            if (arrayAttribute.Contains(type))
            {
                try
                {
                    return Ok(await db_context.TicketManagers.Where(u => (EF.Property<int>(u, type) == data)).ToListAsync());
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return BadRequest("ticket not include that attribute");
        }
    }
}

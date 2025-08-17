using FlightAPIs.Helper;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics.Eventing.Reader;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FlightScheduleController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_context = new DbAbb296Kuphe1980Context();//connection
        public IConfiguration Configuration;//to config data from appsetting 
        public ILogger<FlightScheduleController> Logger;//logging,messager to cli 
        public FlightScheduleController(IConfiguration _configuration , ILogger<FlightScheduleController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //create Flight schedule
        [Authorize(Policy = "roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> flightCreate([FromForm] FlightSchedule flightSchedule)
        {
            //check if there is null field or error 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            //set seat to 30 default
            flightSchedule.TotalSeats = 30;
            flightSchedule.FromAirport = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            flightSchedule.ToAirport = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            flightSchedule.Plane = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            try {
                Dictionary<string,string> catchError = FlightValidation.flightValidation(flightSchedule, db_context, false);
                if(catchError != null)
                {
                    catchError.TryGetValue("Error", out string? errorMessager);
                    return BadRequest(errorMessager);
                }
                flightSchedule.StatusFs = "không hoạt động";//set 
                await db_context.FlightSchedules.AddAsync(flightSchedule);
                await db_context.SaveChangesAsync();
                //import seat 
                FlightSchedule flight = await db_context.FlightSchedules.Where(q => q.Code == flightSchedule.Code.Trim()).FirstOrDefaultAsync();
                for (int number = 1; number <= 30; number++)
                {
                    Seat seats = new Seat();
                    seats.FlightSchedulesId = flight.Id;
                    if(flight.Id == null)
                    {
                        throw new Exception("Error : flight is null");
                    }
                    seats.Seat1 = number.ToString();
                    seats.Isbooked = 0;
                    await db_context.Seats.AddAsync(seats);
                }
                await db_context.SaveChangesAsync();
            }
            catch(SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
            return Ok("create success flight schedule");
        }
        //edit Flight schedule
        [Authorize(Policy = "roleSecurity")]
        [HttpPut]
        public async Task<IActionResult> flightEdit([FromForm] FlightSchedule flightSchedule)
        {
            //check if there is null field or error 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            //set seat to 30 default
            flightSchedule.TotalSeats = 30;
            flightSchedule.FromAirport = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            flightSchedule.ToAirport = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            flightSchedule.Plane = null;//deny create airport bcs ef see it have value of entity Airport -> insert new Airport
            try
            {
                Dictionary<string, string> catchError = FlightValidation.flightValidation(flightSchedule, db_context, true);
                if (catchError != null)
                {
                    catchError.TryGetValue("Error", out string errorMessager);
                    return BadRequest(errorMessager);
                }
                db_context.FlightSchedules.Update(flightSchedule);
                await db_context.SaveChangesAsync();
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
            return Ok(String.Format("update success flight schedule {0}",flightSchedule.Code));
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpDelete]
        public async Task<IActionResult> flightDelete(int? id)
        {
            if(id == null)
            {
                return BadRequest("invalid id to delete flight");
            }
            bool existsFlight = db_context.FlightSchedules.Where(q => q.Id == id).Any();
            if (existsFlight)
            {
                try
                {
                    FlightSchedule defaultFlight = db_context.FlightSchedules.Where(q => q.Id == id).FirstOrDefault();
                    if (defaultFlight == null)
                        return NotFound("FlightSchedule not found");
                    db_context.FlightSchedules.Remove(defaultFlight);
                    await  db_context.SaveChangesAsync();
                }
                catch (DbUpdateException ex) 
                {
                    if (ex.InnerException is SqlException sqlEx)
                    {
                        return BadRequest(sqlEx.Message);
                    }
                    return BadRequest(ex.Message);
                }
                catch (SqlException ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return Ok(String.Format("delete success flight {0}", id));
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> flightAll()
        {
            return Ok(await db_context.FlightSchedules.ToListAsync());
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> flightById(int? id)
        {
            if (id == null)
            {
                return BadRequest("invalid id to find flight");
            }
            FlightSchedule flight;
            try
            {
                flight = await db_context.FlightSchedules.Where(q => q.Id == id).FirstOrDefaultAsync();
            }
            catch(SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
            return Ok(flight);
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> flightByStatus(string status)
        {
            if (status == null)
            {
                return BadRequest("invalid status to find flight");
            }
            List<FlightSchedule> flight;
            try
            {
                flight = await db_context.FlightSchedules.Where(q => q.StatusFs == status.Trim()).ToListAsync();
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
            return Ok(flight);
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> flightByDate(DateTime? departure , DateTime? arrival)
        {
            if (departure == null || arrival == null)
            {
                return BadRequest("invalid DateTime to find flight");
            }
            else if(departure >= arrival)
            {
                return BadRequest("departure must be lower than arrival");
            }
             List<FlightSchedule> flight;
            try
            {
                flight = await db_context.FlightSchedules.Where(q => q.DeparturesAt >= departure && q.ArrivalsAt <= arrival ).ToListAsync();
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
            return Ok(flight);
        }
    }   

}

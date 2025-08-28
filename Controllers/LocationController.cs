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
    public class LocationController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;//to take data from appsetting 
        public ILogger<LocationController> Logger;
        public LocationController(IConfiguration _configuration, ILogger<LocationController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //create 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> create([FromForm] AirPort airport)
        {
            bool airportExist = await db_contex.AirPorts.Where(u => u.Name == airport.Name || u.Code == airport.Code).AnyAsync();
            if (airportExist)
            {
                return BadRequest("Name or Code was exists");
            }
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    airport.flightSchedules = null;
                    airport.flightSchedulesTo = null;
                    await db_contex.AirPorts.AddAsync(airport);
                    await db_contex.SaveChangesAsync();
                    transaction.Commit();
                    return Ok("create airport success");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (ex.InnerException is SqlException exl)
                    {
                        return BadRequest(exl.InnerException!.Message);
                    }
                    return BadRequest(ex.InnerException!.Message);

                }
            }

        }
        //update
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> update([FromForm] AirPort airport)
        {
            bool airportExist = await db_contex.AirPorts.Where(u => (u.Name == airport.Name.Trim() || u.Code == airport.Code.Trim()) && u.Id != airport.Id).AnyAsync();
            if (airportExist)
            {
                return BadRequest("Name or Code was exists");
            }
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    airport.flightSchedules = null;
                    airport.flightSchedulesTo = null;
                    db_contex.AirPorts.Update(airport);
                    await db_contex.SaveChangesAsync();
                    transaction.Commit();
                    return Ok("update airport success");
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
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> delete(int id)
        {
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    db_contex.AirPorts.Remove(await db_contex.AirPorts.FindAsync(id));
                    db_contex.SaveChanges();
                    transaction.Commit();
                    return Ok(string.Format("delete success airport {0}", id));
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
        //read
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> readAll()
        {
            try
            {
                return Ok(await db_contex.AirPorts.ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //read by name 
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> readByName(string name)
        {
            try
            {
                return Ok(await db_contex.AirPorts.Where(u => u.Name.Contains(name)).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //read by code 
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> readByCode(string code)
        {
            try
            {
                return Ok(await db_contex.AirPorts.Where(u => u.Code.Contains(code)).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //read by Id
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> readById(int id)
        {
            try
            {
                return Ok(await db_contex.AirPorts.FindAsync(id));
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
    }
}

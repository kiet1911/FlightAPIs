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
    public class PlanController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_contex = new DbAbb296Kuphe1980Context();
        public IConfiguration Configuration;//to take data from appsetting 
        public ILogger<PlanController> Logger;
        public PlanController(IConfiguration _configuration, ILogger<PlanController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }
        //create 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> create([FromForm] Plane plane)
        {
            bool planeExist = await db_contex.Planes.Where(u => u.Name == plane.Name || u.Code == plane.Code).AnyAsync();
            if (planeExist){
                return BadRequest("Name or Code was exists");
            }
            using(var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    plane.flightSchedules = null;
                    await db_contex.Planes.AddAsync(plane);
                    await db_contex.SaveChangesAsync();
                    transaction.Commit();
                    return Ok("create plane success");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if(ex.InnerException is SqlException exl)
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
        public async Task<IActionResult> update([FromForm] Plane plane)
        {
            bool existNameOrCode = await db_contex.Planes.Where(u => (u.Name == plane.Name.Trim() || u.Code == plane.Code.Trim()) && u.Id != plane.Id).AnyAsync();
            if (existNameOrCode)
            {
                return BadRequest("Name or Code was exists");
            }
            using (var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    plane.flightSchedules = null;
                    db_contex.Planes.Update(plane);
                    await db_contex.SaveChangesAsync();
                    transaction.Commit();
                    return Ok("update plane success");
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
            using(var transaction = db_contex.Database.BeginTransaction())
            {
                try
                {
                    db_contex.Planes.Remove(await db_contex.Planes.FindAsync(id));
                    db_contex.SaveChanges();
                    transaction.Commit();
                    return Ok(string.Format("delete success plane {0}", id));
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
                return Ok(await db_contex.Planes.ToListAsync());
            }
            catch(SqlException ex)
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
                return Ok(await db_contex.Planes.Where(u=>u.Name.Contains(name)).ToListAsync());
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
                return Ok(await db_contex.Planes.Where(u => u.Code.Contains(code)).ToListAsync());
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
                return Ok(await db_contex.Planes.FindAsync(id));
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
    }
}

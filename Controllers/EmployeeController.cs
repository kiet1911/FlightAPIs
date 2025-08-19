using FlightAPIs.Helper;
using FlightAPIs.Internal.DTOdata;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        DbAbb296Kuphe1980Context db = new DbAbb296Kuphe1980Context();
        IConfiguration configuration;
        ILogger<EmployeeController> logger;

        public EmployeeController (IConfiguration _configuration , ILogger<EmployeeController> logger)
        {
            this.configuration = _configuration;
            this.logger = logger;
        }
        //create employee 
        //user DTO employee
        [Authorize(Roles ="Admin")]
        [HttpPost("employee")]
        public async Task<IActionResult> Create([FromForm] EmployeeDTO employeeDTO)
        {
            //modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //validation employee
            EmployeeValidation employeeValidation = new EmployeeValidation();
            Dictionary<string, string> errorMessage = await employeeValidation.validation(db, false, employeeDTO);
            errorMessage.TryGetValue("Error", out string errorrValue);
            if (errorrValue != null)
            {
                return BadRequest(errorrValue);
            }
            //create employee
            Admin admin = new Admin
            {
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                Cccd = employeeDTO.Cccd,
                Address = employeeDTO.Address,
                PhoneNumber = employeeDTO.PhoneNumber,
                Password = employeeDTO.Password,
                UserType = employeeDTO.getUserType()
            };
            //use transaction for rollback 
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //add employee
                   await db.Admins.AddAsync(admin);
                   await db.SaveChangesAsync();
                   transaction.Commit();
                   return Ok("create employee success");
                }
                catch (DbUpdateConcurrencyException ex) {
                    if(ex.InnerException is SqlException exl)
                    {
                        return BadRequest(exl.InnerException.Message);
                    }
                    return BadRequest(ex.InnerException.Message);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        //update
        [Authorize(Roles ="Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> employee([FromForm] EmployeeDTO employeeDTO)
        {
            //modelstate
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //validation employee
            EmployeeValidation employeeValidation = new EmployeeValidation();
            Dictionary<string, string> errorMessage = await employeeValidation.validation(db, true, employeeDTO);
            errorMessage.TryGetValue("Error", out string errorrValue);
            if (errorrValue != null)
            {
                return BadRequest(errorrValue);
            }
            //create employee
            Admin admin = new Admin
            {
                Name = employeeDTO.Name,
                Email = employeeDTO.Email,
                Cccd = employeeDTO.Cccd,
                Address = employeeDTO.Address,
                PhoneNumber = employeeDTO.PhoneNumber,
                Password = employeeDTO.Password,
                UserType = employeeDTO.getUserType()
            };
            //use transaction for rollback 
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    //add employee
                    db.Admins.Update(admin);
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return Ok("update employee success");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (ex.InnerException is SqlException exl)
                    {
                        transaction.Rollback();
                        return BadRequest(exl.InnerException.Message);
                    }
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }

        }
        //can not delete 
        //set to role 3 => role employeeBanned
        [Authorize(Roles = "Admin")]
        [HttpPut("employee")]
        public async Task<IActionResult> RoleChange(int employeeID , int role)
        {
            Admin existsEmployee = await db.Admins.Where(u => u.Id == employeeID).FirstOrDefaultAsync();
            if (existsEmployee == null)
            {
                return BadRequest("invalid employeeID");
            }
            if(role != 3 && role != 0)
            {
                return BadRequest("invalid role");
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    existsEmployee.UserType = role;
                    db.Admins.Update(existsEmployee);
                    db.SaveChanges();
                    transaction.Commit();
                    return Ok("update role for employee success");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if(ex.InnerException is SqlException exl)
                    {
                        transaction.Rollback();
                        return BadRequest(exl.InnerException.Message);
                    }
                    transaction.Rollback();
                    return BadRequest(ex.InnerException.Message);

                }
            } 
        }
        //read 
        //readall 
        [Authorize(Roles = "Admin")]
        [HttpGet("employee")]
        public async Task<IActionResult> readAll()
        {
            try
            {
                return Ok(db.Admins.Where(u=>u.UserType!=2).ToList());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }
        //readby id 
        [Authorize(Roles = "Admin")]
        [HttpGet("employee")]
        public async Task<IActionResult> readById(int id)
        {
            try
            {
                return Ok(await db.Admins.Where(u => u.UserType != 2 && u.Id == id).FirstOrDefaultAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //readby Admintype 
        [Authorize(Roles = "Admin")]
        [HttpGet("employee")]
        public async Task<IActionResult> readByType(int id)
        {
            if(id != 0 && id!= 3)
            {
                return BadRequest("wrong type");
            }
            try
            {
                return Ok(await db.Admins.Where(u=>u.UserType == id).ToListAsync());
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //read by patrition
        [Authorize(Roles = "Admin")]
        [HttpGet("employee")]
        public async Task<IActionResult> readPagition(int pageNumber)
        {
            List<Admin> admins = await db.Admins.OrderBy(u => u.Id).Where(u=>u.UserType != 2).Skip(pageNumber*3).Take(3).ToListAsync();//index clusterd in id admin 
            try
            {
                return Ok(admins);
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        



    }
}

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
        [HttpPost]
        public async Task<IActionResult> employeeCreate([FromForm] EmployeeDTO employeeDTO)
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
        public async Task<IActionResult> employeeUpdate([FromForm] EmployeeDTO employeeDTO)
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
                        return BadRequest(exl.InnerException.Message);
                    }
                    return BadRequest(ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        //can not delete 
        //set to role 3 => role employeeBanned

        //read 
        //readall 
        //readby id 
        //readby type 
        //readby



    }
}

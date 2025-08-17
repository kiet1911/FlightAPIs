using FlightAPIs.Helper;
using FlightAPIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics.Eventing.Reader;

namespace FlightAPIs.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        DbAbb296Kuphe1980Context db_context = new DbAbb296Kuphe1980Context();//connection
        public IConfiguration Configuration;//to config data from appsetting 
        public ILogger<FlightScheduleController> Logger;//logging,messager to cli 
        public PaymentController(IConfiguration _configuration, ILogger<FlightScheduleController> _logger)
        {
            this.Configuration = _configuration;
            this.Logger = _logger;
        }

        //create payment
        [Authorize(Policy ="roleSecurity")]
        [HttpPost]
        public async Task<IActionResult> paymentCreate([FromForm] Payment payment)
        {
            //validation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            //exists User 
            bool userExists = db_context.Users.Where(u => u.Id == payment.UserId).Any();
            if (!userExists)
            {
                return BadRequest("invalid user data");
            }
            if(payment.NamePayment == null | payment.PayerIdPayment == null)
            {
                return BadRequest("name or payId should be fulfill");
            }
            //email pattern
            bool emailPattern = CheckEmail.checkGmail(payment.EmailPayment);
            if (!emailPattern)
            {
                return BadRequest("worng email pattern");
            }
            try
            {
                await db_context.Payments.AddAsync(payment);
                await db_context.SaveChangesAsync();
                return Ok(string.Format("create payment success"));
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }

        }
        //update payment
        [Authorize(Policy = "roleSecurity")]
        [HttpPut]
        public async Task<IActionResult> paymentEdit([FromForm] Payment payment)
        {
            //validation 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.ErrorCount);
            }
            //exists pay 
            bool paymentExists = db_context.Payments.Where(u => u.Id == payment.Id).Any();
            if (!paymentExists)
            {
                return BadRequest("payment does not exists");
            }
            //exists User 
            bool userExists = db_context.Users.Where(u => u.Id == payment.UserId).Any();
            if (!userExists)
            {
                return BadRequest("invalid user data");
            }
            if (payment.NamePayment == null | payment.PayerIdPayment == null)
            {
                return BadRequest("name or payId should be fulfill");
            }
            //email pattern
            bool emailPattern = CheckEmail.checkGmail(payment.EmailPayment);
            if (!emailPattern)
            {
                return BadRequest("worng email pattern");
            }
            try
            {
                db_context.Payments.Update(payment);
                await db_context.SaveChangesAsync();
                return Ok(string.Format("update payment success"));
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
        //update payment
        [Authorize(Policy = "roleSecurity")]
        [HttpDelete]
        public async Task<IActionResult> paymentDelete(int idpayment)
        {
            try
            {
               db_context.Payments.Remove(db_context.Payments.Where(u => u.Id == idpayment).FirstOrDefault());
               await db_context.SaveChangesAsync();
               return Ok("remove payment success");
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
                return BadRequest(ex.InnerException!.Message);
            }
        }
        //read payment
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentAll()
        {
            try
            {
                //
                return Ok(await db_context.Payments.ToListAsync());

            }catch(SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentById(int id)
        {
            try
            {
                //
                return Ok(await db_context.Payments.Where(u=>u.Id == id).FirstOrDefaultAsync());

            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentByUserId(int id)
        {
            try
            {
                //
                return Ok(await db_context.Payments.Where(u => u.UserId == id).ToListAsync());

            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentContainEmail(string text)
        {
            try
            {
                //
                return Ok(await db_context.Payments.Where(u => u.EmailPayment.Contains(text)).ToListAsync());

            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentContainName(string text)
        {
            try
            {
                //
                return Ok(await db_context.Payments.Where(u => u.NamePayment.Contains(text)).ToListAsync());

            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
        [Authorize(Policy = "roleSecurity")]
        [HttpGet]
        public async Task<IActionResult> paymentContainPayer(string text)
        {
            try
            {
                //
                return Ok(await db_context.Payments.Where(u => u.PayerIdPayment.Contains(text)).ToListAsync());

            }
            catch (SqlException ex)
            {
                return BadRequest(ex.InnerException!.Message);
            }
        }
    }
}

using FlightAPIs.Internal.DTOdata;
using FlightAPIs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace FlightAPIs.Helper
{
    public static class TicketValidation
    {
        public static async Task<Dictionary<string,string>> ticketValidation(TicketDTO ticketDTO , DbAbb296Kuphe1980Context _context , bool isCreated)
        {
            Dictionary<string,string> tickError = new Dictionary<string,string>();
            string codeGener = "HN_PQ_200_" + DateTimeOffset.Now.ToUnixTimeSeconds() + (DateTime.Now.Millisecond % 10) ;
            bool ticketPayId = await  _context.Payments.Where(u => u.Id == ticketDTO.PayId && u.UserId == ticketDTO.UserId).AnyAsync();
            if(ticketDTO?.FlightSchedulesId == null)
            {
                tickError.Add("Error", "flightId should not be empty");
                return tickError;
            }
            if(ticketDTO?.UserId == null)
            {
                tickError.Add("Error", "userId should not be empty");
                return tickError;
            }
            bool ticketBookedSeat = await _context.FlightSchedules.Where(u => u.BookedSeats == 30 && u.Id == ticketDTO.FlightSchedulesId).AnyAsync();
            if (ticketBookedSeat) {
                tickError.Add("Error", "filght is full");
                return tickError;
            }
            if (ticketDTO?.Status != 0 && ticketDTO?.Status != 1)
            {
                tickError.Add("Error", "wrong status pattern");
                return tickError;
            }
            if (ticketDTO?.Code == null)
            {
                tickError.Add("Error", "Code should not be empty");
                return tickError;
            }
            if ((ticketDTO?.SeatLocation == null || ticketDTO?.SeatLocation <= 0 || ticketDTO?.SeatLocation > 30 )&& isCreated == false)
            {
                tickError.Add("Error", "invalid seatLocation");
                return tickError;
            }
            if (!ticketPayId)
            {
                tickError.Add("Error", "paymentId not exists");
                return tickError;
            }
            //generate code 
            if (!isCreated)
            {
                ticketDTO.Code = codeGener;
            }
            //return 
            return tickError;
        }
    }
}

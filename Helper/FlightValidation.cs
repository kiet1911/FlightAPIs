using FlightAPIs.Models;

namespace FlightAPIs.Helper
{
    public class FlightValidation
    {
        public static Dictionary<string,string> flightValidation(FlightSchedule flightSchedule , DbAbb296Kuphe1980Context _context , bool flightCreated)
        {
            Dictionary<string, string> flightError = new Dictionary<string, string>();
            //get duration of flight
            TimeSpan timeDuration = flightSchedule.ArrivalsAt - flightSchedule.DeparturesAt;
            //find code 
            bool codeFlight;
            bool existFlight;
            if (!flightCreated)// code check
            {
                codeFlight = _context.FlightSchedules.Where(q => q.Code == flightSchedule.Code).Any();
                if (codeFlight)
                {
                    flightError.Add("Error", "code is exists");
                    return flightError;
                }
            }
            else
            {
                existFlight = _context.FlightSchedules.Where(q => q.Id == flightSchedule.Id).Any();
                if (!existFlight)
                {
                    flightError.Add("Error", "flight is not exists");
                    return flightError;
                }
                codeFlight = _context.FlightSchedules.Where(q => q.Code == flightSchedule.Code && q.Id != flightSchedule.Id).Any();
                if (codeFlight)
                {
                    flightError.Add("Error", "code is exists");
                    return flightError;
                }
            }
            if (flightSchedule.FromAirportId == flightSchedule.ToAirportID)// airport check
            {
                flightError.Add("Error", "location from or to should be differnce from other");
                return flightError;
            }
            if ((flightSchedule.ArrivalsAt < DateTime.Now.AddMinutes(45) || flightSchedule.DeparturesAt < DateTime.Now) && flightCreated == false){
                flightError.Add("Error", "the flight arrival should higher 45 minutes from now");
                return flightError;
            }
            if(timeDuration.TotalHours <= 0.75 || timeDuration.Hours > 3)//duration check
            {
                flightError.Add("Error", "the flight departures should higher 45 minutes and below 2 hour");
                    return flightError; ;
            }
            if(flightSchedule.Cost > 5000000 || flightSchedule.Cost < 0)// cost check
            {
                flightError.Add("Error", "the cost to buy ticket should below 5 million vnd");
                return flightError;
            }
            if (flightSchedule.StatusFs != "đang hoạt động" && flightSchedule.StatusFs != "không hoạt động")
            {
                flightError.Add("Error", "wrong status pattern");
                return flightError;
            }
            return null; ;
        }
    }
}

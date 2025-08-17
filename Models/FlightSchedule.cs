using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class FlightSchedule
{
    public int Id { get; set; }

    public int PlaneId { get; set; }
    public virtual Plane Plane { get; set; }

    public int FromAirportId { get; set; }
    public virtual AirPort FromAirport { get; set; }

    public int ToAirportID { get; set; }
    public virtual AirPort ToAirport { get; set; }

    public DateTime DeparturesAt { get; set; }

    public DateTime ArrivalsAt { get; set; }

    public int Cost { get; set; }

    public string Code { get; set; } = null!;

    public int? TotalSeats { get; set; }

    public string? StatusFs { get; set; }

    public int? BookedSeats { get; set; }

    public int? AvailableSeats { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<TicketManager> TicketManagers { get; set; } = new List<TicketManager>();
    //public virtual AirPort fromAirport { get; set; } = null;
}

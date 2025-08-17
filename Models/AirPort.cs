using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class AirPort
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Address { get; set; } = null!;
    public virtual ICollection<FlightSchedule> flightSchedules { get; set; } = new List<FlightSchedule>();
    public virtual ICollection<FlightSchedule> flightSchedulesTo { get; set; } = new List<FlightSchedule>();
}

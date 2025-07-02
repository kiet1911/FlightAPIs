using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class Seat
{
    public int Id { get; set; }

    public int FlightSchedulesId { get; set; }

    public string? Seat1 { get; set; }

    public int? Isbooked { get; set; }

    public byte[] Version { get; set; } = null!;

    public DateTime? BookingExpiration { get; set; }

    public virtual FlightSchedule FlightSchedules { get; set; } = null!;
}

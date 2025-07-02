using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class TicketManager
{
    public int Id { get; set; }

    public int FlightSchedulesId { get; set; }

    public int UserId { get; set; }

    public int Status { get; set; }

    public string Code { get; set; } = null!;

    public int? SeatLocation { get; set; }

    public int? PayId { get; set; }

    public virtual FlightSchedule FlightSchedules { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

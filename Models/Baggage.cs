using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class Baggage
{
    public int Id { get; set; }

    public int CarryonBag { get; set; }

    public int SignedLuggage { get; set; }

    public string Code { get; set; } = null!;

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}

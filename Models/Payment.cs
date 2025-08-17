using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string? EmailPayment { get; set; }

    public string? NamePayment { get; set; }

    public string? PayerIdPayment { get; set; }

    public int? UserId { get; set; }
    //public virtual User? user { get; set; } = null;
}

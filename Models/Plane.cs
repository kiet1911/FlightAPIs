using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class Plane
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}

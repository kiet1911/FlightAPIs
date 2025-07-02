using System;
using System.Collections.Generic;

namespace FlightAPIs.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; } = null!;

    public string? Cccd { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public int UserType { get; set; }

    public virtual ICollection<Baggage> Baggages { get; set; } = new List<Baggage>();

    public virtual ICollection<TicketManager> TicketManagers { get; set; } = new List<TicketManager>();
}

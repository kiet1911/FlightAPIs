namespace FlightAPIs.Internal.DTOdata
{
    public class FlightDTO
    {
        public int PlaneId { get; set; }

        public int FromAirport { get; set; }

        public int ToAirport { get; set; }

        public DateTime DeparturesAt { get; set; }

        public DateTime ArrivalsAt { get; set; }

        public int Cost { get; set; }

        public string Code { get; set; } = null!;

        public int? TotalSeats { get; set; }

        public string? StatusFs { get; set; }

        public int? BookedSeats { get; set; }

        public int? AvailableSeats { get; set; }
    }
}

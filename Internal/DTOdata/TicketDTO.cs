namespace FlightAPIs.Internal.DTOdata
{
    sealed public class TicketDTO
    {
        public TicketDTO() { }
        public int Id { get; set; }

        public int FlightSchedulesId { get; set; }

        public int UserId { get; set; }

        public int Status { get; set; }

        public string Code { get; set; } = null!;

        public int? SeatLocation { get; set; }

        public int? PayId { get; set; }
    }
}

namespace ApecMoviePortal.Models
{
    public class TicketViewModel
    {
        public Guid MovieID { get; set; }
        public int SeatNumber { get; set; }
        public DateTime ShowTime { get; set; }
    }
}

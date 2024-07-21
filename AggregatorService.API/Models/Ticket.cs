namespace AggregatorService.API.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public Guid MovieID { get; set; }
        public int SeatNumber { get; set; }
        public DateTime ShowTime { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? Token { get; set; }
        public Guid? UserID { get; set; }
    }

    public enum PaymentStatus
    {
        Paid,
        Unpaid,
        Cancelled
    }
}

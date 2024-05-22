using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ApecMoviePortal.Models
{
    public class Ticket
    {
        [JsonProperty("movieID")]
        public Guid MovieID { get; set; }

        [JsonProperty("seatNumber")]
        public int SeatNumber { get; set; }

        [JsonProperty("showTime")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime ShowTime { get; set; }

        [JsonProperty("paymentStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentStatus PaymentStatus { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("userID")]
        public Guid UserID { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }
    }

    public enum PaymentStatus
    {
        Paid = 0,
        Unpaid = 1,
        Cancelled = 2
    }

}

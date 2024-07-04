namespace LogServices.API.Models
{
    public class LogStatistics
    {
        public long TotalLogs { get; set; }
        public Dictionary<string, long> LogsByLevel { get; set; }
    }
}

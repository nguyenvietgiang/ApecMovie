using LogServices.API.Models;

namespace LogServices.API.Services
{
    public interface ILogService
    {
        Task<List<Log>> GetLogsAsync(string collectionName, int pageNumber, int pageSize);
        Task<Log> GetLogByIdAsync(string collectionName, string id);
        Task<List<Log>> SearchLogsAsync(string collectionName, string keyword, DateTime? startDate, DateTime? endDate);
        Task<LogStatistics> GetStatisticsAsync(string collectionName);
        Task CreateLogAsync(string collectionName, Log log);
        Task DeleteLogAsync(string collectionName, string id);
    }

}

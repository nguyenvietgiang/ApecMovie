using LogServices.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LogServices.API.Services
{
    public class LogService : ILogService
    {
        private readonly IMongoDatabase _database;

        public LogService(IMongoDatabase database)
        {
            _database = database;
        }

        private async Task<IMongoCollection<Log>> GetOrCreateCollectionAsync(string collectionName)
        {
            collectionName = collectionName.ToLowerInvariant();
            var collections = await _database.ListCollectionNamesAsync();
            var existingCollectionName = await collections.ToListAsync()
                .ContinueWith(t => t.Result.FirstOrDefault(name => name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)));

            if (existingCollectionName != null)
            {
                return _database.GetCollection<Log>(existingCollectionName);
            }

            // Nếu collection không tồn tại, tạo mới
            await _database.CreateCollectionAsync(collectionName);
            return _database.GetCollection<Log>(collectionName);
        }

        public async Task<List<Log>> GetLogsAsync(string collectionName, int pageNumber, int pageSize)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            return await collection.Find(Builders<Log>.Filter.Empty)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Limit(pageSize)
                                   .ToListAsync();
        }

        public async Task<Log> GetLogByIdAsync(string collectionName, string id)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            return await collection.Find(Builders<Log>.Filter.Eq(log => log.Id, ObjectId.Parse(id)))
                                   .FirstOrDefaultAsync();
        }

        public async Task<List<Log>> SearchLogsAsync(string collectionName, string keyword, DateTime? startDate, DateTime? endDate)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            var filterBuilder = Builders<Log>.Filter;
            var filters = new List<FilterDefinition<Log>>();

            if (!string.IsNullOrEmpty(keyword))
            {
                filters.Add(filterBuilder.Text(keyword));
            }
            if (startDate.HasValue)
            {
                filters.Add(filterBuilder.Gte(log => log.Timestamp, startDate.Value));
            }
            if (endDate.HasValue)
            {
                filters.Add(filterBuilder.Lte(log => log.Timestamp, endDate.Value));
            }

            var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<LogStatistics> GetStatisticsAsync(string collectionName)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            var totalLogs = await collection.CountDocumentsAsync(Builders<Log>.Filter.Empty);
            var levelGroups = await collection.Aggregate()
                                              .Group(log => log.Level, g => new { Level = g.Key, Count = g.Count() })
                                              .ToListAsync();
            var statistics = new LogStatistics
            {
                TotalLogs = totalLogs,
                LogsByLevel = levelGroups.ToDictionary(g => g.Level, g => (long)g.Count)
            };
            return statistics;
        }

        public async Task CreateLogAsync(string collectionName, Log log)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            await collection.InsertOneAsync(log);
        }

        public async Task DeleteLogAsync(string collectionName, string id)
        {
            var collection = await GetOrCreateCollectionAsync(collectionName);
            await collection.DeleteOneAsync(Builders<Log>.Filter.Eq(log => log.Id, ObjectId.Parse(id)));
        }
    }
}

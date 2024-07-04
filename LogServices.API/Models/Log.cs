using MongoDB.Bson;
using System.Globalization;

namespace LogServices.API.Models
{
    public class Log
    {
        public ObjectId? Id { get; set; }
        public DateTime Timestamp { get; set; } // Giữ nguyên kiểu DateTime
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public string RenderedMessage { get; set; }
        public Dictionary<string, object>? Properties { get; set; }

        public DateTime ParseTimestampFromString(string timestampString)
        {
            // Logic xử lý chuyển đổi chuỗi timestamp sang DateTime
            // Bạn có thể sử dụng thư viện DateTime.ParseExact hoặc các phương thức khác
            return DateTime.ParseExact(timestampString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // Định dạng cần điều chỉnh cho phù hợp với MongoDB
        }
    }

}

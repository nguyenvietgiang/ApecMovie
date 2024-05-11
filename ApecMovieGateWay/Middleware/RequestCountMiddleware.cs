using System.Collections.Concurrent;

namespace ApecMovieGateWay.Middleware
{
    public class RequestCountMiddleware
    {
    private static ConcurrentDictionary<string, int> RequestCounts = new ConcurrentDictionary<string, int>();
        private readonly RequestDelegate _next;

        public RequestCountMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Lấy thông tin về downstream service từ URL hoặc RouteValues
            var downstreamService = context.Request.Path.Value;

            // Tăng số lượng request được xử lý cho downstream service này
            RequestCounts.AddOrUpdate(downstreamService, 1, (key, value) => value + 1);

            // Chuyển request đến middleware tiếp theo trong pipeline
            await _next(context);
        }

        public static void LogRequestCounts()
        {
            Console.WriteLine("Request counts for downstream services:");
            foreach (var kvp in RequestCounts)
            {
                Console.WriteLine($"Service: {kvp.Key}, Requests: {kvp.Value}");
            }
        }
    }
}

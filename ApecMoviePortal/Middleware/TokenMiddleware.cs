namespace ApecMoviePortal.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("AccessToken"))
            {
                var token = context.Request.Cookies["AccessToken"];
                context.Request.Headers.Authorization = "Bearer " + token;
            }

            await _next(context);
        }

    }
}

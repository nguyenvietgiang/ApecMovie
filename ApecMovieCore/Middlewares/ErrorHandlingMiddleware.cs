using ApecMovieCore.BaseResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ApecMovieCore.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // log lỗi vào conssole
                Console.WriteLine($"An error occurred: {ex}");

                // Handle những lỗi thường gặp (400,401,500)
                var statusCode = StatusCodes.Status500InternalServerError;
                var message = $"An error occurred: {ex}";

                if (ex is NotImplementedException)
                {
                    statusCode = StatusCodes.Status501NotImplemented;
                    message = "Not Implemented";
                }
                else if (ex is UnauthorizedAccessException)
                {
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Unauthorized";
                }
                else if (ex is ValidationException validationException)
                {
                    // Lỗi do validate
                    statusCode = StatusCodes.Status400BadRequest;
                    message = "One or more validation errors occurred.";
                }

                // Tạo dl trả về khi sảy ra lỗi
                var response = new Response<object>(statusCode, message, null);
                var jsonResponse = JsonConvert.SerializeObject(response);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}


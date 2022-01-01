using System;
using ers_server_net6_minapi.Models;

namespace ers_server_net6_minapi.Middleware {

    public class ApiKeyMiddleware {
        private readonly RequestDelegate _next;
        private string APIKEYNAME = "x-ers-api-key";
        public ApiKeyMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext) {

            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey)) {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api token was not provided or is invalid. ");
                return;
            }

            await _next(context);
        }
    }
}


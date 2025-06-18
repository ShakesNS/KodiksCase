using KodiksCase.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Middleware
{
    // Middleware that globally catches unhandled exceptions, logs them with correlation IDs, and returns standardized error responses.
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to handle requests and globally catch exceptions.
        /// </summary>
        /// <param name="context">HTTP context of the current request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Retrieve existing correlation ID or generate a new one for tracing
                var correlationId = context.Items["X-Correlation-ID"]?.ToString() ?? Guid.NewGuid().ToString();

                // Log the exception with correlation ID for troubleshooting
                logger.LogError(ex, "Unhandled exception caught. CorrelationId: {CorrelationId}", correlationId);

                // Set the response content type and status code for error response
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // Create a standardized error response including the correlation ID
                var response = new ErrorResponse("An unexpected error occurred. Please try again later.")
                {
                    // İstersen correlationId'yi de mesaj içine ekleyebilirsin:
                    Errors = new List<string> { $"CorrelationId: {correlationId}" }
                };

                // Serialize the error response to JSON
                var json = JsonSerializer.Serialize(response);

                // Write the JSON error response to the HTTP response body
                await context.Response.WriteAsync(json);
            }
        }
    }
}

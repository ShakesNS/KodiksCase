using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Infrastructure.Middleware
{
    // Middleware that logs all incoming HTTP requests and their responses with correlation IDs.
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ApiLoggingMiddleware> logger;
        private const string HeaderKey = "X-Correlation-ID";

        /// <summary>
        /// Initializes the middleware with the next request delegate and a logger.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">Logger to record request and response information.</param>
        public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Processes the HTTP context to log request details and response status,
        /// while generating or retrieving a correlation ID for tracking.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request has a correlation ID header; if not, generate a new GUID
            var correlationId = context.Request.Headers.ContainsKey(HeaderKey)
                ? context.Request.Headers[HeaderKey].ToString()
                : Guid.NewGuid().ToString();

            // Store the correlation ID in HttpContext.Items for access in other middleware or controllers
            context.Items[HeaderKey] = correlationId;

            // Add the correlation ID to Serilog's logging context so it appears in all logs within this scope
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                // Capture HTTP method, path, query string, and remote IP address for logging
                var method = context.Request.Method;
                var path = context.Request.Path;
                var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();

                // Log the incoming request details along with the correlation ID
                logger.LogInformation("Incoming Request: {Method} {Path}{QueryString} from {RemoteIpAddress} - CorrelationId: {CorrelationId}",
                    method, path, queryString, remoteIp, correlationId);

                // Invoke the next middleware in the pipeline
                await next(context);

                // After response is generated, log the response status code with correlation ID
                var statusCode = context.Response.StatusCode;
                logger.LogInformation("Response Status Code: {StatusCode} for {Method} {Path} - CorrelationId: {CorrelationId}",
                    statusCode, method, path, correlationId);
            }
        }
    }
}

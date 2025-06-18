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
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ApiLoggingMiddleware> logger;
        private const string HeaderKey = "X-Correlation-ID";

        public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // CorrelationId al veya oluştur
            var correlationId = context.Request.Headers.ContainsKey(HeaderKey)
                ? context.Request.Headers[HeaderKey].ToString()
                : Guid.NewGuid().ToString();

            // Context içine koy (diğer middleware/metodlar erişebilir)
            context.Items[HeaderKey] = correlationId;

            // Serilog context'e ekle (Loglarda görünür)
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                var method = context.Request.Method;
                var path = context.Request.Path;
                var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
                var remoteIp = context.Connection.RemoteIpAddress?.ToString();

                logger.LogInformation("Incoming Request: {Method} {Path}{QueryString} from {RemoteIpAddress} - CorrelationId: {CorrelationId}",
                    method, path, queryString, remoteIp, correlationId);

                await next(context);

                var statusCode = context.Response.StatusCode;
                logger.LogInformation("Response Status Code: {StatusCode} for {Method} {Path} - CorrelationId: {CorrelationId}",
                    statusCode, method, path, correlationId);
            }
        }
    }
}

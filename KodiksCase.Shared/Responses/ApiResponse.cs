using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Operation completed successfully.";
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data, string? message = null)
        {
            Data = data;
            if (!string.IsNullOrWhiteSpace(message))
                Message = message;
        }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>(data, message);
        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message
            };
        }
    }
}

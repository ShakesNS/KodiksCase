using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodiksCase.Shared.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; }
        public List<string>? Errors { get; set; }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse(string message, List<string> errors)
        {
            Message = message;
            Errors = errors;
        }
    }
}

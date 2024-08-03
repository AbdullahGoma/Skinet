using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiException : ApiResponse
    {
        public ApiException(int statusCode, string message = null, string deatails = null) : base(statusCode, message)
        {
            Details = deatails;
        }

        public string Details { get; set; }
    }
}
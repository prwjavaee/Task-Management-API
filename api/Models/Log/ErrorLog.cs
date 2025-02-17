using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Log
{
    public class ErrorLog
    {
        public int Id { get; set; }

        public string? AppUserId { get; set; }
        public string? AppUserName { get; set; }

        public string? Uri { get; set; }          
        public string? HttpMethod { get; set; }    
        public int? ErrorCode { get; set; }
        public string? ErrorType { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }    
    }
}
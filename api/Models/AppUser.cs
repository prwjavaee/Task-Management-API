using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Log;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser : IdentityUser
    {
        public List<WorkOrder> WorkOrder { get; set; } = new List<WorkOrder>();
        public List<ApiLog> ApiLog { get; set; } = new List<ApiLog>();
    }
}
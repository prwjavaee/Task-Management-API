using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Log;

namespace api.Interfaces
{
    public interface IApiLogRepository
    {
        Task<List<ApiLog>> GetAllAsync();
        Task<ApiLog> CreateAsync(ApiLog apiLog);
    }
}
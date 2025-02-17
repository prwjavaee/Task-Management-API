using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Log;

namespace api.Interfaces
{
    public interface IErrorLogRepository
    {
        Task<List<ErrorLog>> GetAllAsync();
        Task<ErrorLog> CreateAsync(ErrorLog errorLog);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models.Log;
using Microsoft.EntityFrameworkCore;

namespace api.Repository.Log
{
    public class ApiLogRepository : IApiLogRepository
    {
        private readonly ApplicationDBContext _context;
        public ApiLogRepository(ApplicationDBContext context)
        {
            this._context = context;
        }

        public async Task<ApiLog> CreateAsync(ApiLog apiLog)
        {
            await _context.ApiLogs.AddAsync(apiLog);
            await _context.SaveChangesAsync();
            return apiLog;
        }

        public async Task<List<ApiLog>> GetAllAsync()
        {
            var apiLogs = await _context.ApiLogs.ToListAsync();
            return apiLogs;
        }
    }
}
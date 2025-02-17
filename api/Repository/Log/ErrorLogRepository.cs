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
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly ApplicationDBContext _context;
        public ErrorLogRepository(ApplicationDBContext context)
        {
            this._context = context;
        }

        public async Task<ErrorLog> CreateAsync(ErrorLog errorLog)
        {
            await _context.ErrorLogs.AddAsync(errorLog);
            await _context.SaveChangesAsync();
            return errorLog;
        }

        public async Task<List<ErrorLog>> GetAllAsync()
        {
            var apiLogs = await _context.ErrorLogs.ToListAsync();
            return apiLogs;
        }


        
    }
}
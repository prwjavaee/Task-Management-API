using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.WorkOrder;
using api.Interfaces;
using api.Models;
using api.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly ApplicationDBContext _context;

        public WorkOrderRepository(ApplicationDBContext context)
        {
            this._context = context;
        }

        public async Task<WorkOrder> CreateAsync(WorkOrder workOrder)
        {
            await _context.WorkOrders.AddAsync(workOrder);
            await _context.SaveChangesAsync();
            return workOrder;
        }

        public async Task<WorkOrder?> DeleteAsync(int id)
        {
            var workOrder = await _context.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
            if (workOrder == null) return null;
            _context.Remove(workOrder);
            await _context.SaveChangesAsync();
            return workOrder;
        }

        public async Task<List<WorkOrder>> GetAllAsync(WorkOrderQueryObject queryObject, AppUser appUser)
        {
            var workOrders = _context.WorkOrders.AsQueryable();

            if (appUser != null)
            {
                workOrders = workOrders.Where(w => w.AppUserId == appUser.Id);
            }
            if (!string.IsNullOrWhiteSpace(queryObject.Title)){
                workOrders = workOrders.Where(w => w.Title.Contains(queryObject.Title));
            }
            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if (queryObject.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                {
                    workOrders = queryObject.IsDecsending ? workOrders.OrderByDescending(w => w.Title) : workOrders.OrderBy(w => w.Title);
                }
                if (queryObject.SortBy.Equals("EndDate", StringComparison.OrdinalIgnoreCase))
                {
                    workOrders = queryObject.IsDecsending ? workOrders.OrderByDescending(w => w.EndDate) : workOrders.OrderBy(w => w.EndDate);
                }
            }
            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;
            return await workOrders.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<WorkOrder?> GetByIdAsync(int id)
        {
            // include => Eager Loading / Entity Framework Core 中的預先載入方法。
            return await _context.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<WorkOrder?> UpdateAsync(int id, WorkOrderUpdateRequestDto workOrderDto)
        {
            var WorkOrder = await _context.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
            if(WorkOrder == null) return null;

            WorkOrder.Title = workOrderDto.Title;
            WorkOrder.Description = workOrderDto.Description;
            WorkOrder.StartDate = workOrderDto.StartDate;
            WorkOrder.EndDate = workOrderDto.EndDate;
            WorkOrder.IsCompleted = workOrderDto.IsCompleted;

            await _context.SaveChangesAsync();
            return WorkOrder;
        }
    }
}
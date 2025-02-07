using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.WorkOrder;
using api.Models;
using api.QueryObjects;

namespace api.Interfaces
{
    public interface IWorkOrderRepository
    {
        Task<List<WorkOrder>> GetAllAsync(WorkOrderQueryObject queryObject, AppUser appUser);
        Task<WorkOrder?> GetByIdAsync(int id);
        Task<WorkOrder> CreateAsync(WorkOrder workOrder);
        Task<WorkOrder?> UpdateAsync(int id, WorkOrderUpdateRequestDto workOrderDto);
        Task<WorkOrder?> DeleteAsync(int id);
    }
}
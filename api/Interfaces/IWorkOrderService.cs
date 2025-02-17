using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.QueryObjects;

namespace api.Interfaces
{
    public interface IWorkOrderService
    {
        Task<List<WorkOrder>> GetAllAsync(WorkOrderQueryObject queryObject, AppUser appUser);
        Task ClearWorkOrderCache();
    }
}
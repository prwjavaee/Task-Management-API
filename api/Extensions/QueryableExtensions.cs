using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.QueryObjects;

namespace api.Extensions
{
    public static  class QueryableExtensions
    {
        public static IQueryable<WorkOrder> ApplyQuery(this IQueryable<WorkOrder> workOrders, WorkOrderQueryObject queryObject, AppUser appUser)
        {
            if (appUser != null)
            {
                workOrders = workOrders.Where(w => w.AppUserId == appUser.Id);
            }
            
            if (!string.IsNullOrWhiteSpace(queryObject.Title))
            {
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
            workOrders = workOrders.Skip(skipNumber).Take(queryObject.PageSize);

            return workOrders;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.QueryObjects;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace api.Service
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IWorkOrderRepository _workOrderRepo;

        public WorkOrderService(IConnectionMultiplexer redis, IWorkOrderRepository workOrderRepo)
        {
            _redis = redis;
            _workOrderRepo = workOrderRepo;
        }

        public async Task<List<WorkOrder>> GetAllAsync(WorkOrderQueryObject query, AppUser user)
        {
            var db = _redis.GetDatabase();

            var cacheKey = $"WorkOrder_GetAll_{user.Id}";
            var cacheQueryKey = $"{cacheKey}_Query";

            string cachedData = await db.StringGetAsync(cacheKey);
            string cachedQueryJson = await db.StringGetAsync(cacheQueryKey);

            List<WorkOrder> workOrders;
            WorkOrderQueryObject cachedQuery = null;

            // 注意：針對Controller設定的循環引用處理在這裡不起效，
            // 所以我們需要在Service中另外設定或使用全域設定(Program.cs)來避免循環引用。
            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            if (!string.IsNullOrEmpty(cachedData) && !string.IsNullOrEmpty(cachedQueryJson))
            {
                // Deserialize 快取資料
                workOrders = JsonConvert.DeserializeObject<List<WorkOrder>>(cachedData,serializerSettings);
                cachedQuery = JsonConvert.DeserializeObject<WorkOrderQueryObject>(cachedQueryJson);
                
                // 只有排序條件變更
                if (IsOnlySortingChanged(query, cachedQuery))
                {
                    return workOrders.AsQueryable().ApplySorting(query).ToList();
                }
            }

            workOrders = await _workOrderRepo.GetAllAsync(query, user);
            await db.StringSetAsync(cacheKey, JsonConvert.SerializeObject(workOrders,serializerSettings), TimeSpan.FromMinutes(30));
            await db.StringSetAsync(cacheQueryKey, JsonConvert.SerializeObject(query), TimeSpan.FromMinutes(30));
            return workOrders.AsQueryable().ApplyQuery(query, user).ToList();

        }

        public async Task ClearWorkOrderCache(AppUser user)
        {
            var db = _redis.GetDatabase();
            var cacheKey = $"WorkOrder_GetAll_{user.Id}";
            var cacheQueryKey = $"{cacheKey}_Query";
            await db.KeyDeleteAsync(cacheKey);
            await db.KeyDeleteAsync(cacheQueryKey);
        }

        // 確保條件是"排序"而不是"篩選"
        private bool IsOnlySortingChanged(WorkOrderQueryObject newQuery, WorkOrderQueryObject cachedQuery)
        {
            // 檢查篩選條件是否改變
            bool filterUnchanged = newQuery.Title == cachedQuery.Title &&
                                    newQuery.PageNumber == cachedQuery.PageNumber &&
                                    newQuery.PageSize == cachedQuery.PageSize;

            // 若篩選條件沒改，並且只有排序條件改變，則返回 true
            bool sortingChanged = newQuery.SortBy != cachedQuery.SortBy ||
                                newQuery.IsDecsending != cachedQuery.IsDecsending;

            // 當篩選條件沒變動且排序條件變動時，或者完全沒變動，都返回 true
            return filterUnchanged && !sortingChanged;
        }

    }
}
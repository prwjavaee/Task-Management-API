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
            string cachedData = await db.StringGetAsync(cacheKey);
            List<WorkOrder> workOrders;

            // 注意：針對Controller設定的循環引用處理在這裡不起效，
            // 所以我們需要在Service中另外設定或使用全域設定(Program.cs)來避免循環引用。
            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Deserialize 快取資料
                workOrders = JsonConvert.DeserializeObject<List<WorkOrder>>(cachedData,serializerSettings);
                return workOrders.AsQueryable().ApplyQuery(query, user).ToList();
            }
            else
            {
                workOrders = await _workOrderRepo.GetAllAsync(query, user);
                // 序列化後快取，設定 TTL
                await db.StringSetAsync(cacheKey, JsonConvert.SerializeObject(workOrders,serializerSettings), TimeSpan.FromMinutes(30));
                return workOrders;
            }
        }

        public async Task ClearWorkOrderCache(AppUser user)
        {
            var db = _redis.GetDatabase();
            var cacheKey = $"WorkOrder_GetAll_{user.Id}";
            await db.KeyDeleteAsync(cacheKey);
        }
        
    }
}
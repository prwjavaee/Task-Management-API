using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.WorkOrder;
using api.Models;

namespace api.Mappers
{
    //靜態類別 不需要建立實例就可以直接使用其成員。
    public static class WorkOrderMapper
    {

        //靜態方法 直接被呼叫不需要先實例化 StockMappers 類別 ; this(Extension Method) 可以更自然使用.ToStockDto() 不使用則需要加入參數變成.ToStockDto(someStock)
        public static WorkOrderDto ToWorkOrderDto(this WorkOrder workOrder)
        {
            return new WorkOrderDto
            {
                Id = workOrder.Id,
                Title = workOrder.Title,
                Description = workOrder.Description,
                StartDate = workOrder.StartDate,
                EndDate = workOrder.EndDate,
                IsCompleted = workOrder.IsCompleted
            };
        }
        
        public static WorkOrder ToWorkOrderFromCreateDto(this WorkOrderCreateRequestDto workOrderDto)
        {
            return new WorkOrder
            {
                Title = workOrderDto.Title,
                Description = workOrderDto.Description,
                EndDate = workOrderDto.EndDate,
                IsCompleted = workOrderDto.IsCompleted
            };
        }
    }
}
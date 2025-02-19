using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.WorkOrder;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.QueryObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ServiceFilter(typeof(LogActionFilter))]
    [Route("api/WorkOrder")]
    [ApiController] // Web API 的輔助屬性 自動模型驗證&自動綁定來源
    [Authorize] //需要Token
    public class WorkOrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IWorkOrderRepository _workOrderRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWorkOrderService _workOrderService;

        public WorkOrderController(ApplicationDBContext context,IWorkOrderRepository workOrderRepo,UserManager<AppUser> userManager,IWorkOrderService workOrderService)
        {
            _workOrderRepo = workOrderRepo;
            _context = context;
            _userManager = userManager;
            _workOrderService = workOrderService;
        }

        private async Task<AppUser> GetCurrentUserAsync()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            return appUser;
        }

        [AllowAnonymous] // 允許匿名 公開這個api 不須token
        [ServiceFilter(typeof(LogActionFilter), IsReusable = false)]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is working!");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] WorkOrderQueryObject query)
        {
            //ModelState 請求參數是否符合定義 有上attribute[]的都會檢查
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var appUser = await GetCurrentUserAsync();
            var workOrder = await _workOrderService.GetAllAsync(query,appUser);
            var workOrderDto = workOrder.Select(w => w.ToWorkOrderDto()).ToList();
            return Ok(workOrderDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var workOrder = await _workOrderRepo.GetByIdAsync(id);
            if (workOrder == null) return NotFound();
            return Ok(workOrder.ToWorkOrderDto());
        }

        [Authorize(Policy = "AuthorizedUserOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkOrderCreateRequestDto CreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var workOrder = CreateDto.ToWorkOrderFromCreateDto();
            var appUser = await GetCurrentUserAsync();
            workOrder.AppUser = appUser;
            await _workOrderRepo.CreateAsync(workOrder);
            await _workOrderService.ClearWorkOrderCache(appUser);
            // ActionName , RouteValue , Obj ; 導向方法，方法參數，回傳類型
            return CreatedAtAction(nameof(GetById), new { id = workOrder.Id }, workOrder.ToWorkOrderDto());
            // return Created();
        }

        [Authorize(Policy = "AuthorizedUserOnly")]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] WorkOrderUpdateRequestDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var workOrder = await _workOrderRepo.UpdateAsync(id, updateDto);
            if (workOrder == null) return NotFound();
            var appUser = await GetCurrentUserAsync();
            await _workOrderService.ClearWorkOrderCache(appUser);
            return Ok(workOrder.ToWorkOrderDto());
        }

        [Authorize(Policy = "AuthorizedUserOnly")]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var workOrder = await _workOrderRepo.DeleteAsync(id);
            if (workOrder == null) return NotFound();
            var appUser = await GetCurrentUserAsync();
            await _workOrderService.ClearWorkOrderCache(appUser);
            return NoContent();
        }

    }
}
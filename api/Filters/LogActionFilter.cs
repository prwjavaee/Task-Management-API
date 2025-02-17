using System.Text;
using System.Text.Json;
using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Models.Log;
using api.Repository.Log;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LogActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;
    private readonly IApiLogRepository _apiLogRepository;
    private readonly UserManager<AppUser> _userManager;

    public LogActionFilter(ILogger<LogActionFilter> logger,IApiLogRepository apiLogRepository,UserManager<AppUser> userManager)
    {
        _logger = logger;
        _apiLogRepository = apiLogRepository;
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ApiLog apiLog = new ApiLog();
        // Action Before
        await HandleBeforeActionAsync(context,apiLog);
        // 呼叫 Action（執行後返回的結果封裝在 ActionExecutedContext）
        var executedContext = await next();
        // Action After
        await HandleAfterActionAsync(executedContext,apiLog);
    }

    private async Task<ApiLog> HandleBeforeActionAsync(ActionExecutingContext context, ApiLog apiLog)
    {
        // Extensions 需要解析token
        var username = context.HttpContext.User.GetUsername();
        var actionName = context.ActionDescriptor.DisplayName;  
        _logger.LogInformation($"🔍 [Before Action] Action: {actionName}, User: {username}");

        // AppUser
        var appUser = await _userManager.FindByNameAsync(username.ToLower());
        apiLog.AppUserId = appUser?.Id;
        apiLog.AppUserName = appUser?.UserName;

        // Request
        var request = context.HttpContext.Request;
        apiLog.Uri = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        apiLog.HttpMethod = request.Method;

        // RequestData
        apiLog.RequestData = JsonSerializer.Serialize(context.ActionArguments);

        // 紀錄時間
        apiLog.Timestamp = DateTime.UtcNow;
        return apiLog;
    }

    private async Task<ApiLog> HandleAfterActionAsync(ActionExecutedContext context, ApiLog apiLog)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var statusCode = context.HttpContext.Response.StatusCode;
        _logger.LogInformation($"✅ [After Action] Action: {actionName}, Status Code: {statusCode}");

        // ResponseData
        if (context.Result is ObjectResult objectResult)
        {
            apiLog.ResponseData = JsonSerializer.Serialize(objectResult.Value);
        }
        
        await _apiLogRepository.CreateAsync(apiLog); // 儲存 Log 到資料庫
        return apiLog;
    }

}


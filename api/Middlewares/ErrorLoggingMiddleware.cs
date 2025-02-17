using api.Extensions;
using api.Interfaces;
using api.Models;
using api.Models.Log;
using api.Repository.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;
    // private ErrorLog errorLog;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        // 不要在這裡初始化 errorLog，因為中間件的實例會在整個應用程序生命周期內持續存在。
        // 如果在這裡初始化，errorLog 會在所有請求間共享，並且無法針對每個請求獨立處理錯誤。
        // errorLog = new ErrorLog(); 致命錯誤
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var errorLog = new ErrorLog();
        try
        {
            // Input
            errorLog = await HandleInputAsync(context, serviceProvider,errorLog);
            // 執行下一個中間件 
            await _next(context);
            // 認證與授權失敗 (沒token的遊客跟沒權限的一般會員)
            if (context.Items.ContainsKey("ErrorLog"))
            {
                string username = "guest";
                if (context.User.Identity.IsAuthenticated) username = context.User.GetUsername();
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var appUser = await userManager.FindByNameAsync(username.ToLower());
                errorLog.AppUserId = appUser?.Id;
                errorLog.AppUserName = appUser?.UserName ?? username;

                var errorLogItem = context.Items["ErrorLog"] as ErrorLog;
                errorLog.ErrorCode = errorLogItem?.ErrorCode;
                errorLog.ErrorType = errorLogItem?.ErrorType;
                errorLog.ErrorMessage = errorLogItem?.ErrorMessage;
                // 記錄錯誤
                var serializedLog = JsonSerializer.Serialize(errorLog, new JsonSerializerOptions { WriteIndented = true });
                _logger.LogError($"🔍🔍🔍🔍🔍🔍{serializedLog}🔍🔍🔍🔍🔍🔍");
                // 儲存到資料庫
                var errorLogRepository = serviceProvider.GetRequiredService<IErrorLogRepository>();
                await errorLogRepository.CreateAsync(errorLog);
            }
        }
        catch (Exception ex)
        {
            // 額外未知錯誤
            errorLog.ErrorCode = StatusCodes.Status500InternalServerError;
            errorLog.ErrorType = "Exception";
            errorLog.ErrorMessage = ex.Message;

            var serializedLog = JsonSerializer.Serialize(errorLog, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogError($"🔥🔥🔥 Exception: {serializedLog} 🔥🔥🔥");

            var errorLogRepository = serviceProvider.GetRequiredService<IErrorLogRepository>();
            await errorLogRepository.CreateAsync(errorLog);

            // 返回標準化錯誤格式
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new 
            {
                error = "Internal Server Error",
                message = "發生未預期的錯誤，請稍後再試。",
                errorCode = StatusCodes.Status500InternalServerError
            });
            await context.Response.WriteAsync(result);
        }

    }

    private async Task<ErrorLog> HandleInputAsync(HttpContext context, IServiceProvider serviceProvider, ErrorLog errorLog)
    {
        var apiUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        errorLog.Uri = apiUrl;
        errorLog.HttpMethod = context.Request.Method;
        errorLog.Timestamp = DateTime.UtcNow;
        return errorLog;
    }

    // private async Task HandleOutputAsync(HttpContext context, IServiceProvider serviceProvider, MemoryStream memoryStream)
    // {
    //     // 未來擴充
    // }

}

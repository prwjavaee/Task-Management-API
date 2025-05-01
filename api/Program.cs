using api.Data;
using api.Interfaces;
using api.Models;
using api.Models.Log;
using api.Repository;
using api.Repository.Log;
using api.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//  OpenAPI 規範（Swagger）服務擴展方法
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Swagger
builder.Services.AddSwaggerGen();
// Swagger 啟用JWT驗證功能
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


// 避免循環引用 **只針對Controller
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// 連線字串
// builder.Services.AddDbContext<ApplicationDBContext>(options =>
// {
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
// });
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsProduction())
{
    connectionString = connectionString.Replace("localhost", "sqlserver");
}
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    // ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"))
    ConnectionMultiplexer.Connect("redis:6379,abortConnect=false")
);

// Identity 註冊身份驗證機制 (使用者 & 角色管理)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 0;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

// JWT 設定身份驗證機制 (Token)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => // 設定 JWT 驗證細節
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse(); // 阻止預設的 `WWW-Authenticate` 回應
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "需要有效的 Token" });
            context.Response.WriteAsync(result);

            context.HttpContext.Items["ErrorLog"] = new ErrorLog
            {
                ErrorCode = StatusCodes.Status401Unauthorized,
                ErrorType = "Unauthorized",
                ErrorMessage = "需要有效的 Token",
            };
            return Task.CompletedTask; //繼續流向後續的中間件，不會因為認證失敗中斷
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = "您的權限不足" });
            context.Response.WriteAsync(result);

            context.HttpContext.Items["ErrorLog"] = new ErrorLog
            {
                ErrorCode = StatusCodes.Status403Forbidden,
                ErrorType = "Forbidden",
                ErrorMessage = "您的權限不足",
            };
            return Task.CompletedTask;            
        }
    };
});

// 授權
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    // Claim 中自定義的授權
    options.AddPolicy("PermissionEdit", policy => policy.RequireClaim("Permission", "Edit"));
    options.AddPolicy("PermissionDelete", policy => policy.RequireClaim("Permission", "Delete"));
});


// DI 不需要手動建立依賴物件，而是由框架自動提供
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IApiLogRepository, ApiLogRepository>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var context = services.GetRequiredService<ApplicationDBContext>();

    try
    {
        // 確保資料庫遷移
        context.Database.Migrate();
        // 創建 Admin 用戶
        ApplicationDBContext.SeedAdminUserAsync(userManager).Wait(); // 非同步方法需要 Wait()，但要確保它不阻塞主執行緒    
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup Error] 資料庫初始化失敗：{ex.Message}");
    }

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//錯誤日誌
app.UseMiddleware<ErrorLoggingMiddleware>();
//驗證
app.UseAuthentication();
//授權
app.UseAuthorization();

app.MapControllers();

app.Run();


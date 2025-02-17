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
<<<<<<< HEAD
=======
using StackExchange.Redis;
>>>>>>> 9a5308c (feat: add Redis caching for WorkOrder GetAll)

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

<<<<<<< HEAD
// 避免循環引用
=======
// 避免循環引用 **只針對Controller
>>>>>>> 9a5308c (feat: add Redis caching for WorkOrder GetAll)
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// 連線字串
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
<<<<<<< HEAD
=======
// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"))
);
>>>>>>> 9a5308c (feat: add Redis caching for WorkOrder GetAll)

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
    // Claim 自定義授權
    options.AddPolicy("AuthorizedUserOnly", policy => policy.RequireClaim("Permission", "Edit"));
});


// DI 不需要手動建立依賴物件，而是由框架自動提供
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IApiLogRepository, ApiLogRepository>();
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
<<<<<<< HEAD
=======
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

>>>>>>> 9a5308c (feat: add Redis caching for WorkOrder GetAll)


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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


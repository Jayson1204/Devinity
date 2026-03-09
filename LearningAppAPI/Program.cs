using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using LearningApp.Api.Data;
using LearningApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Learning App API",
        Version = "v1",
        Description = "Learning App API with JWT Authentication"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── Database ──────────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

// ── JWT ───────────────────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── Rate Limiting ─────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("auth", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 5;
        o.QueueLimit = 0;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("api", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 60;
        o.QueueLimit = 2;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("static", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 200;
        o.QueueLimit = 0;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(1),
                PermitLimit = 100,
                QueueLimit = 0
            }));

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            """{"error":"Too many requests. Please slow down and try again."}""",
            token);
    };
});

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IAvatarService, AvatarService>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Ensure upload directory exists on startup
Directory.CreateDirectory("/app/uploads/avatars");

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Learning App API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

// Serve avatar images from Railway volume as static files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("/app/uploads"),
    RequestPath = "/uploads"
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health").RequireRateLimiting("static");

app.MapGet("/info", () => new
{
    message = "Learning App API",
    version = "1.0.0",
    status = "running",
    timestamp = DateTime.UtcNow
}).RequireRateLimiting("static");

// ── Run ───────────────────────────────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
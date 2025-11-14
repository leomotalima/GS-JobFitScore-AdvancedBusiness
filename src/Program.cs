using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using JobFitScoreAPI.Data;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Swagger;
using DotNetEnv;
using Microsoft.ML;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName != "Testing")
    Env.Load();

var user = Environment.GetEnvironmentVariable("ORACLE_USER_ID");
var pass = Environment.GetEnvironmentVariable("ORACLE_PASSWORD");
var dataSource = Environment.GetEnvironmentVariable("ORACLE_DATA_SOURCE");
var connectionString = $"User Id={user};Password={pass};Data Source={dataSource};";

if (builder.Environment.EnvironmentName != "Testing")
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseOracle(connectionString));
    }
}

builder.Services.AddSingleton(new MLContext());
builder.Services.AddScoped<JobFitMLService>();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var key = Encoding.UTF8.GetBytes(
    builder.Environment.EnvironmentName == "Testing"
        ? "testing_key_123"
        : builder.Configuration["Jwt:Key"] ?? "default_key_12345"
);

builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "JobFitScore API", Version = "v1" });
    opt.SwaggerDoc("v2", new OpenApiInfo { Title = "JobFitScore API", Version = "v2" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opt.OperationFilter<SwaggerSecurityRequirementsFilter>();
    opt.OperationFilter<SwaggerAllowAnonymousFilter>();
    opt.DocumentFilter<Documentacao>();
    opt.DocumentFilter<OrdenarTagsDocumentFilter>();
});

var hc = builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("BancoOracle");

if (builder.Environment.EnvironmentName != "Testing")
{
    hc.AddUrlGroup(
        new Uri("https://api.github.com/"),
        name: "API externa - GitHub",
        failureStatus: HealthStatus.Degraded
    );
}

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"JobFitScore API {description.GroupName.ToUpper()}"
            );
        }
        options.RoutePrefix = "swagger";
    });
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health/ping", (IHostEnvironment env) =>
{
    var start = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
    var uptime = DateTime.UtcNow - start;

    return Results.Ok(new
    {
        success = true,
        message = "API rodando com sucesso",
        data = new
        {
            status = "Healthy",
            version = "1.0.0",
            uptime = uptime.ToString(@"hh\:mm\:ss"),
            environment = env.EnvironmentName,
            host = Environment.MachineName,
            timestampUtc = DateTime.UtcNow
        },
        statusCode = 200,
        timestampUtc = DateTime.UtcNow
    });
});

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        var uptime = DateTime.UtcNow - startTime;

        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();

        var result = new
        {
            success = true,
            message = "Health check executado com sucesso",
            data = new
            {
                status = report.Status.ToString(),
                version = "1.0.0",
                uptime = uptime.ToString(@"hh\:mm\:ss"),
                environment = env.EnvironmentName,
                host = Environment.MachineName,
                timestampUtc = DateTime.UtcNow,
                checks = report.Entries.Select(e => new
                {
                    componente = e.Key,
                    status = e.Value.Status.ToString(),
                    descricao = e.Value.Description
                })
            },
            statusCode = context.Response.StatusCode,
            timestampUtc = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
});

app.MapControllers();
app.Run();

public partial class Program { }

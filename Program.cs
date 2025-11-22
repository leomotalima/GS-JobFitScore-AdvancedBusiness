using Asp.Versioning;
using JobFitScoreAPI;
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
using JobFitScoreAPI.Repositories;
using JobFitScoreAPI.Repository;
using JobFitScoreAPI.Repository.Interfaces;
using DotNetEnv;
using Microsoft.ML;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Logging estruturado
// ----------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ----------------------
// Carregar .env (exceto em teste)
// ----------------------
if (builder.Environment.EnvironmentName != "Testing")
    Env.Load();

// ----------------------
// Banco de Dados Oracle
// ----------------------
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__OracleConnection");

if (builder.Environment.EnvironmentName != "Testing")
{
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Connection string Oracle não encontrada!");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString));
}

// ----------------------
// ML.NET
// ----------------------
builder.Services.AddSingleton(new MLContext());
builder.Services.AddScoped<JobFitMLService>();

// ----------------------
// Repositories
// ----------------------
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IHabilidadeRepository, HabilidadeRepository>();

// ----------------------
// Services
// ----------------------
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CandidaturaService>();
builder.Services.AddScoped<VagaService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<HabilidadeService>();
builder.Services.AddScoped<UsuarioHabilidadeService>();
builder.Services.AddScoped<VagaHabilidadeService>();
builder.Services.AddScoped<CursoService>();

// ----------------------
// Controllers
// ----------------------
builder.Services.AddControllers();

// ----------------------
// Versionamento da API
// ----------------------
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

// ----------------------
// JWT
// ----------------------
var key = Encoding.UTF8.GetBytes(
    builder.Environment.EnvironmentName == "Testing"
        ? "testing_key_123"
        : builder.Configuration["Jwt:Key"] ?? "default_key_12345"
);

var jwtKey = builder.Configuration["Jwt:Key"] 
             ?? "ChaveSuperUltraMegaSeguraComMaisDe32Caracteres_123456";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "JobFitScore";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "JobFitScoreUsers";
var jwtExpireMinutes = 120;

builder.Services.AddSingleton(sp => new JwtService(
    jwtKey,
    jwtIssuer,
    jwtAudience,
    jwtExpireMinutes
));
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

// ----------------------
// Swagger
// ----------------------
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
        In = ParameterLocation.Header,
        Description = "Insira o token JWT(tokenAcesso)"
    });

    opt.OperationFilter<SwaggerSecurityRequirementsFilter>();
    opt.OperationFilter<SwaggerAllowAnonymousFilter>();
    opt.DocumentFilter<OrdenarTagsDocumentFilter>();
    opt.EnableAnnotations();
});

// ----------------------
// Health Check
// ----------------------
var hc = builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("BancoOracle");

builder.Services.AddAuthorization();

// ----------------------
// OpenTelemetry Tracing
// ----------------------
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("JobFitScoreAPI"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

// ----------------------
// Build
// ----------------------
var app = builder.Build();

// ----------------------
// Swagger no ambiente Dev
// ----------------------
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

// ----------------------
// Redirect root → Swagger
// ----------------------
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// ----------------------
// Auth
// ----------------------
app.UseAuthentication();
app.UseAuthorization();

// ----------------------
// Health Ping
// ----------------------
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

// ----------------------
// Health Check Completo
// ----------------------
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

// ----------------------
// Controllers
// ----------------------
app.MapControllers();

// ----------------------
// Run
// ----------------------
app.Run();

public partial class Program { }
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

using JobFitScoreAPI.Data;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Repository;
using JobFitScoreAPI.Repository.Interfaces;
using JobFitScoreAPI.Swagger;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Logging
// ----------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ----------------------
// Carregar .env
// ----------------------
if (builder.Environment.EnvironmentName != "Testing")
    Env.Load();

// ----------------------
// Banco de Dados Oracle
// ----------------------
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__OracleConnection");

if (builder.Environment.EnvironmentName != "Testing" && !string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString));
}
else if (builder.Environment.EnvironmentName != "Testing")
{
    throw new InvalidOperationException("Connection string Oracle não encontrada!");
}

// ----------------------
// Machine Learning
// ----------------------
builder.Services.AddSingleton(new MLContext());
builder.Services.AddScoped<JobFitMLService>();

// ----------------------
// Repositories
// ----------------------
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICandidaturaRepository, CandidaturaRepository>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();

// ----------------------
// Versionamento da API
// ----------------------
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = false;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ----------------------
// Autenticação JWT
// ----------------------
var key = Encoding.UTF8.GetBytes(
    builder.Environment.EnvironmentName == "Testing"
        ? "testing_key_123"
        : builder.Configuration["Jwt:Key"] ?? "default_key_12345"
);

builder.Services.AddSingleton<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JobFitScore API",
        Version = "v1",
        Description = "API de avaliação de vagas e candidatos com IA"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Envie o token no formato: Bearer {seu_token}"
    });

    options.OperationFilter<SwaggerSecurityRequirementsFilter>();
    options.OperationFilter<SwaggerAllowAnonymousFilter>();
    options.DocumentFilter<Documentacao>();
    options.DocumentFilter<OrdenarTagsDocumentFilter>();
    options.EnableAnnotations();
});

// ----------------------
// Health Checks
// ----------------------
builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString!,
        name: "banco-oracle",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded,
        tags: new[] { "db", "oracle" },
        timeout: TimeSpan.FromSeconds(5)
    );

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

// ----------------------
// Controllers
// ----------------------
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

// ----------------------
// OpenTelemetry
// ----------------------
builder.Services.AddOpenTelemetry()
    .WithTracing(tp =>
    {
        tp.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("JobFitScoreAPI"))
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation()
          .AddConsoleExporter();
    });

// ----------------------
// Build app
// ----------------------
var app = builder.Build();

// ----------------------
// Swagger Dev
// ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobFitScore API v1");
        c.RoutePrefix = "swagger";
    });
}

// ----------------------
// Redirect root → Swagger
// ----------------------
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// ----------------------
// Auth
// ----------------------
app.UseAuthentication();
app.UseAuthorization();

// ----------------------
// Health Checks endpoints
// ----------------------
app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/api/health/db", new HealthCheckOptions
{
    Predicate = c => c.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

// ----------------------
// Controllers
// ----------------------
app.MapControllers();

app.Run();

public partial class Program { }

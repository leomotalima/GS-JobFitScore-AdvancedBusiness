using Asp.Versioning.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using JobFitScoreAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JobFitScoreAPI.Services;
using JobFitScoreAPI.Swagger;
using DotNetEnv;
using Microsoft.ML;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// BANCO DE DADOS
// ============================================================
if (builder.Environment.EnvironmentName != "Testing")
    Env.Load();

var user = Environment.GetEnvironmentVariable("ORACLE_USER_ID");
var pass = Environment.GetEnvironmentVariable("ORACLE_PASSWORD");
var dataSource = Environment.GetEnvironmentVariable("ORACLE_DATA_SOURCE");

var connectionString = $"User Id={user};Password={pass};Data Source={dataSource};";

if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString));
}
else
{
    throw new InvalidOperationException("Connection string Oracle não encontrada!");
}

// ============================================================
// MACHINE LEARNING
// ============================================================
builder.Services.AddSingleton(new MLContext());
builder.Services.AddSingleton<JobFitMlService>();

// ============================================================
// VERSIONAMENTO DA API
// ============================================================
builder.Services
    .AddApiVersioning(o =>
    {
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.DefaultApiVersion = new ApiVersion(1, 0);
        o.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


// ============================================================
// AUTENTICAÇÃO JWT
// ============================================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default_key_12345");
builder.Services.AddSingleton<JwtService>(); // Garante que a classe exista

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ============================================================
// SWAGGER
// ============================================================
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JobFitScore API",
        Version = "v1",
        Description = "API para análise de compatibilidade profissional"
    });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token no formato: Bearer {token}"
    });

    opt.OperationFilter<SwaggerSecurityRequirementsFilter>();
    opt.OperationFilter<SwaggerAllowAnonymousFilter>();
    opt.DocumentFilter<Documentacao>();
    opt.DocumentFilter<OrdenarTagsDocumentFilter>();
    // 🔹 REMOVIDO opt.EnableAnnotations(); pois o pacote não está instalado
});

// ============================================================
// HEALTH CHECKS
// ============================================================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("BancoOracle")
    .AddUrlGroup(
        new Uri("https://api.github.com/"),
        name: "API externa - GitHub",
        failureStatus: HealthStatus.Degraded
    );

// ============================================================
// CONTROLLERS E SERVIÇOS
// ============================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

// ============================================================
// PIPELINE
// ============================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobFitScore API v1");
        c.RoutePrefix = "swagger";
    });
}

// Redireciona raiz para Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// HEALTH CHECKS ENDPOINTS
// ============================================================
app.MapGet("/api/health/ping", () => Results.Ok(new
{
    status = "API rodando",
    timestamp = DateTime.UtcNow
}));

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                componente = e.Key,
                status = e.Value.Status.ToString(),
                descricao = e.Value.Description
            }),
            data = DateTime.UtcNow
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.MapControllers();
app.Run();

public partial class Program { }

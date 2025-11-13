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
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// VARIÁVEIS DE AMBIENTE (Oracle, JWT etc.)
// ============================================================
if (builder.Environment.EnvironmentName != "Testing")
    Env.Load();

var user = Environment.GetEnvironmentVariable("ORACLE_USER_ID");
var pass = Environment.GetEnvironmentVariable("ORACLE_PASSWORD");
var dataSource = Environment.GetEnvironmentVariable("ORACLE_DATA_SOURCE");
var connectionString = $"User Id={user};Password={pass};Data Source={dataSource};";

// ============================================================
// BANCO DE DADOS ORACLE
// ============================================================
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
// MACHINE LEARNING (ML.NET)
// ============================================================
builder.Services.AddSingleton(new MLContext());
builder.Services.AddScoped<JobFitMLService>();

// ============================================================
// CONTROLLERS
// ============================================================
builder.Services.AddControllers();

// ============================================================
// VERSIONAMENTO DA API (v1 e v2) – Asp.Versioning 8.1
// ============================================================
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

// ============================================================
// AUTENTICAÇÃO JWT
// ============================================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default_key_12345");

builder.Services.AddSingleton<JwtService>();

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
// SWAGGER (com versionamento dinâmico)
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JobFitScore API",
        Version = "v1",
        Description = "API para análise de compatibilidade profissional - Versão 1 (CRUDs e dados base)"
    });

    opt.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "JobFitScore API",
        Version = "v2",
        Description = "API para análise de compatibilidade profissional - Versão 2 (IA e métricas)"
    });

    // Removido opt.EnableAnnotations(); — opcional e incompatível com versão atual

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
});

// ============================================================
// HEALTH CHECKS (API + Banco + Externo)
// ============================================================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("BancoOracle")
    .AddUrlGroup(
        new Uri("https://api.github.com/"),
        name: "API externa - GitHub",
        failureStatus: HealthStatus.Degraded
    );

// ============================================================
// AUTORIZAÇÃO
// ============================================================
builder.Services.AddAuthorization();

// ============================================================
// PIPELINE
// ============================================================
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

// Redireciona raiz para o Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Segurança
app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// ENDPOINTS DE HEALTH CHECK
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

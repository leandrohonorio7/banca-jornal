using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;
using BancaJornal.Repository.Repositories;
using BancaJornal.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger com suporte a PathBase
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banca Jornal API",
        Version = "v1",
        Description = "API para gerenciamento de banca de jornal",
        Contact = new OpenApiContact
        {
            Name = "Banca Jornal",
            Email = "contato@bancajornal.com"
        }
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<BancaJornalDbContext>("database");

// Configure DbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=/app/data/bancajornal.db";
builder.Services.AddDbContext<BancaJornalDbContext>(options =>
    options.UseSqlite(connectionString));

// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();

// Register application services
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<DashboardService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        
        if (allowedOrigins.Length == 1 && allowedOrigins[0] == "*")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

// Configurar PathBase para rota /bancajornal
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

// Configure the HTTP request pipeline
app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}";
        if (!string.IsNullOrEmpty(pathBase))
        {
            serverUrl += pathBase;
        }
        swagger.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = serverUrl, Description = "Banca Jornal API" }
        };
    });
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "Banca Jornal API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Banca Jornal API";
});

// Servir arquivos estáticos do Blazor WASM
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();

app.UseCors("ProductionPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();

// Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

app.MapControllers();

// Fallback para SPA - redirecionar todas as rotas não-API para index.html
app.MapFallbackToFile("index.html");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BancaJornalDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

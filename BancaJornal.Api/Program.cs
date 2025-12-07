using Microsoft.EntityFrameworkCore;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;
using BancaJornal.Repository.Repositories;
using BancaJornal.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Servir arquivos estáticos do Blazor WASM
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();

app.UseCors("ProductionPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
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

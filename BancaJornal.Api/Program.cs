using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BancaJornal.Repository;
using BancaJornal.Application;
using BancaJornal.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<BancaJornalDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
// ... outros serviços
var app = builder.Build();
app.MapControllers();
app.Run();

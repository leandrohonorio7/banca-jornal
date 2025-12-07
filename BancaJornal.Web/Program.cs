using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using BancaJornal.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<BancaJornal.Web.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient com a URL base da API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddBlazoredLocalStorage();

// Registrar ViewModels
builder.Services.AddTransient<ProdutoViewModel>();
builder.Services.AddTransient<VendaViewModel>();

// Registrar Services
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<ProdutoIndexedDbService>();
builder.Services.AddScoped<VendaIndexedDbService>();
builder.Services.AddScoped<ProdutoSyncService>();
builder.Services.AddScoped<VendaSyncService>();

await builder.Build().RunAsync();

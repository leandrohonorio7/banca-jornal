using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BancaJornal.Web;
using CommunityToolkit.Mvvm.DependencyInjection;
using Blazored.LocalStorage;
using Blazor.IndexedDB.Framework;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddIndexedDB(options =>
{
    options.DbName = "BancaJornalCache";
    options.Version = 1;
    options.Stores.Add(new StoreSchema
    {
        Name = "produtos",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false },
        Indexes = new List<IndexSpec> {
            new IndexSpec { Name = "nome", KeyPath = "nome", Auto = false }
        }
    });
    options.Stores.Add(new StoreSchema
    {
        Name = "vendas",
        PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false },
        Indexes = new List<IndexSpec> {
            new IndexSpec { Name = "data", KeyPath = "data", Auto = false }
        }
    });
});
// DI para ViewModels e Services
builder.Services.AddTransient<ProdutoViewModel>();
builder.Services.AddTransient<VendaViewModel>();
builder.Services.AddScoped<ProdutoIndexedDbService>();
builder.Services.AddScoped<VendaIndexedDbService>();
builder.Services.AddScoped<ProdutoSyncService>();
builder.Services.AddScoped<VendaSyncService>();
// ... outros serviços
await builder.Build().RunAsync();

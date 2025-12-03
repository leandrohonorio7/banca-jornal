using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using BancaJornal.Application.Services;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;
using BancaJornal.Repository.Repositories;
using BancaJornal.Desktop.Views;
using BancaJornal.Desktop.ViewModels;

namespace BancaJornal.Desktop;

/// <summary>
/// Aplicação WPF com configuração de injeção de dependências.
/// Aplica DIP - todas as dependências são injetadas via container.
/// </summary>
public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        // Configuração do DbContext com SQLite
        services.AddDbContext<BancaJornalDbContext>(options =>
            options.UseSqlite("Data Source=/app/bancajornal.db"));

        // Registro de repositórios e Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();

        // Registro de serviços de aplicação
        services.AddScoped<ProdutoService>();
        services.AddScoped<VendaService>();
        services.AddScoped<DashboardService>();

        // Registro de ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ProdutoViewModel>();
        services.AddTransient<VendaViewModel>();

        // Registro de Views
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Criar banco de dados se não existir
        using (var scope = _serviceProvider!.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BancaJornalDbContext>();
            dbContext.Database.EnsureCreated();
        }

        // Exibir janela principal
        var mainWindow = _serviceProvider!.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}

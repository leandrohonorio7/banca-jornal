# Guia Rápido de Desenvolvimento

## 🚀 Setup Inicial

```powershell
# Clone/navegue até o projeto
cd c:\Users\leandro.honorio.lima\source\repo-ia

# Restaurar pacotes NuGet
dotnet restore BancaJornal.sln

# Compilar
dotnet build BancaJornal.sln

# Executar
dotnet run --project BancaJornal.Desktop
```

## 📝 Comandos Úteis

### Build e Execução
```powershell
# Build completo
dotnet build BancaJornal.sln --configuration Release

# Limpar build
dotnet clean BancaJornal.sln

# Executar projeto Desktop
dotnet run --project BancaJornal.Desktop

# Executar com hot reload (se disponível)
dotnet watch run --project BancaJornal.Desktop
```

### Entity Framework (Migrations)
```powershell
# Navegar até o projeto Repository
cd BancaJornal.Repository

# Adicionar nova migration
dotnet ef migrations add NomeDaMigration --startup-project ../BancaJornal.Desktop

# Atualizar banco de dados
dotnet ef database update --startup-project ../BancaJornal.Desktop

# Remover última migration
dotnet ef migrations remove --startup-project ../BancaJornal.Desktop

# Ver SQL gerado
dotnet ef migrations script --startup-project ../BancaJornal.Desktop

# Voltar ao diretório raiz
cd ..
```

## 🔨 Adicionar Nova Funcionalidade

### 1. Nova Entidade de Domínio

**Arquivo:** `BancaJornal.Model/Entities/MinhaEntidade.cs`

```csharp
namespace BancaJornal.Model.Entities;

public class MinhaEntidade
{
    public int Id { get; private set; }
    
    public MinhaEntidade(string nome)
    {
        ValidarNome(nome);
        Nome = nome;
    }
    
    public void Atualizar(string nome)
    {
        ValidarNome(nome);
        Nome = nome;
    }
    
    private void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.", nameof(nome));
    }
}
```

### 2. Interface do Repositório

**Arquivo:** `BancaJornal.Repository/Interfaces/IMinhaEntidadeRepository.cs`

```csharp
using BancaJornal.Model.Entities;

namespace BancaJornal.Repository.Interfaces;

public interface IMinhaEntidadeRepository
{
    Task<MinhaEntidade?> ObterPorIdAsync(int id);
    Task<IEnumerable<MinhaEntidade>> ObterTodosAsync();
    Task AdicionarAsync(MinhaEntidade entidade);
    Task AtualizarAsync(MinhaEntidade entidade);
    Task RemoverAsync(int id);
}
```

### 3. Implementação do Repositório

**Arquivo:** `BancaJornal.Repository/Repositories/MinhaEntidadeRepository.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Data;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Repository.Repositories;

public class MinhaEntidadeRepository : IMinhaEntidadeRepository
{
    private readonly BancaJornalDbContext _context;

    public MinhaEntidadeRepository(BancaJornalDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<MinhaEntidade?> ObterPorIdAsync(int id)
    {
        return await _context.MinhasEntidades
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<MinhaEntidade>> ObterTodosAsync()
    {
        return await _context.MinhasEntidades
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AdicionarAsync(MinhaEntidade entidade)
    {
        await _context.MinhasEntidades.AddAsync(entidade);
    }

    public async Task AtualizarAsync(MinhaEntidade entidade)
    {
        _context.MinhasEntidades.Update(entidade);
        await Task.CompletedTask;
    }

    public async Task RemoverAsync(int id)
    {
        var entidade = await _context.MinhasEntidades.FindAsync(id);
        if (entidade != null)
            _context.MinhasEntidades.Remove(entidade);
    }
}
```

### 4. Atualizar DbContext

**Arquivo:** `BancaJornal.Repository/Data/BancaJornalDbContext.cs`

```csharp
// Adicionar DbSet
public DbSet<MinhaEntidade> MinhasEntidades { get; set; }

// Adicionar configuração em OnModelCreating
modelBuilder.Entity<MinhaEntidade>(entity =>
{
    entity.ToTable("MinhasEntidades");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
});
```

### 5. Atualizar Unit of Work

**Arquivo:** `BancaJornal.Repository/Interfaces/IUnitOfWork.cs`

```csharp
public interface IUnitOfWork : IDisposable
{
    IProdutoRepository Produtos { get; }
    IVendaRepository Vendas { get; }
    IMinhaEntidadeRepository MinhasEntidades { get; } // NOVO
    
    Task<int> CommitAsync();
    Task RollbackAsync();
}
```

**Arquivo:** `BancaJornal.Repository/Repositories/UnitOfWork.cs`

```csharp
private IMinhaEntidadeRepository? _minhaEntidadeRepository;

public IMinhaEntidadeRepository MinhasEntidades
{
    get
    {
        _minhaEntidadeRepository ??= new MinhaEntidadeRepository(_context);
        return _minhaEntidadeRepository;
    }
}
```

### 6. Criar DTO

**Arquivo:** `BancaJornal.Application/DTOs/MinhaEntidadeDto.cs`

```csharp
using BancaJornal.Model.Entities;

namespace BancaJornal.Application.DTOs;

public class MinhaEntidadeDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;

    public static MinhaEntidadeDto FromEntity(MinhaEntidade entidade)
    {
        return new MinhaEntidadeDto
        {
            Id = entidade.Id,
            Nome = entidade.Nome
        };
    }
}
```

### 7. Criar Service

**Arquivo:** `BancaJornal.Application/Services/MinhaEntidadeService.cs`

```csharp
using BancaJornal.Application.DTOs;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Application.Services;

public class MinhaEntidadeService
{
    private readonly IUnitOfWork _unitOfWork;

    public MinhaEntidadeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<MinhaEntidadeDto?> ObterPorIdAsync(int id)
    {
        var entidade = await _unitOfWork.MinhasEntidades.ObterPorIdAsync(id);
        return entidade != null ? MinhaEntidadeDto.FromEntity(entidade) : null;
    }

    public async Task<IEnumerable<MinhaEntidadeDto>> ObterTodosAsync()
    {
        var entidades = await _unitOfWork.MinhasEntidades.ObterTodosAsync();
        return entidades.Select(MinhaEntidadeDto.FromEntity);
    }

    public async Task<MinhaEntidadeDto> CriarAsync(string nome)
    {
        var entidade = new MinhaEntidade(nome);
        await _unitOfWork.MinhasEntidades.AdicionarAsync(entidade);
        await _unitOfWork.CommitAsync();
        return MinhaEntidadeDto.FromEntity(entidade);
    }

    public async Task<MinhaEntidadeDto> AtualizarAsync(int id, string nome)
    {
        var entidade = await _unitOfWork.MinhasEntidades.ObterPorIdAsync(id);
        if (entidade == null)
            throw new InvalidOperationException("Entidade não encontrada.");

        entidade.Atualizar(nome);
        await _unitOfWork.MinhasEntidades.AtualizarAsync(entidade);
        await _unitOfWork.CommitAsync();
        return MinhaEntidadeDto.FromEntity(entidade);
    }
}
```

### 8. Criar ViewModel

**Arquivo:** `BancaJornal.Desktop/ViewModels/MinhaEntidadeViewModel.cs`

```csharp
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

namespace BancaJornal.Desktop.ViewModels;

public partial class MinhaEntidadeViewModel : ObservableObject
{
    private readonly MinhaEntidadeService _service;

    [ObservableProperty]
    private ObservableCollection<MinhaEntidadeDto> _entidades = new();

    [ObservableProperty]
    private string _nome = string.Empty;

    public MinhaEntidadeViewModel(MinhaEntidadeService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task Carregar()
    {
        try
        {
            var lista = await _service.ObterTodosAsync();
            Entidades.Clear();
            foreach (var item in lista)
            {
                Entidades.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Salvar()
    {
        try
        {
            await _service.CriarAsync(Nome);
            MessageBox.Show("Sucesso!", "Sucesso", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            Nome = string.Empty;
            await Carregar();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
```

### 9. Criar View (XAML)

**Arquivo:** `BancaJornal.Desktop/Views/MinhaEntidadeView.xaml`

```xml
<UserControl x:Class="BancaJornal.Desktop.Views.MinhaEntidadeView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0">
            <TextBox Text="{Binding Nome, UpdateSourceTrigger=PropertyChanged}" 
                     PlaceholderText="Nome..."/>
            <Button Content="Salvar" Command="{Binding SalvarCommand}"/>
        </StackPanel>

        <DataGrid Grid.Row="1" ItemsSource="{Binding Entidades}">
            <DataGrid.Columns>
                <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="50"/>
                <DataGridTextColumn Header="Nome" Binding="{Binding Nome}" Width="*"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</UserControl>
```

**Arquivo:** `BancaJornal.Desktop/Views/MinhaEntidadeView.xaml.cs`

```csharp
using System.Windows.Controls;

namespace BancaJornal.Desktop.Views;

public partial class MinhaEntidadeView : UserControl
{
    public MinhaEntidadeView()
    {
        InitializeComponent();
    }
}
```

### 10. Registrar Injeção de Dependências

**Arquivo:** `BancaJornal.Desktop/App.xaml.cs`

```csharp
// Em ConfigureServices(), adicionar:
services.AddScoped<IMinhaEntidadeRepository, MinhaEntidadeRepository>();
services.AddScoped<MinhaEntidadeService>();
services.AddTransient<MinhaEntidadeViewModel>();
```

### 11. Executar Migration

```powershell
cd BancaJornal.Repository
dotnet ef migrations add AdicionarMinhaEntidade --startup-project ../BancaJornal.Desktop
dotnet ef database update --startup-project ../BancaJornal.Desktop
cd ..
```

## 🐛 Debugging

### Visual Studio
1. Abrir `BancaJornal.sln`
2. Definir `BancaJornal.Desktop` como projeto de inicialização
3. Pressionar F5 ou Ctrl+F5

### VS Code
1. Instalar extensão C# Dev Kit
2. Abrir pasta do projeto
3. F5 para debug

### Logs de EF Core
Adicionar em `BancaJornalDbContext`:
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
}
```

## 📊 Verificar Estrutura do Banco

```powershell
# Instalar SQLite Tools (se não tiver)
dotnet tool install --global dotnet-sqlite

# Abrir banco
sqlite3 bancajornal.db

# Comandos SQLite
.tables              # Listar tabelas
.schema Produtos     # Ver estrutura da tabela
SELECT * FROM Produtos LIMIT 10;
.quit                # Sair
```

## 🧪 Testes (Para Implementar)

```powershell
# Criar projeto de testes
dotnet new xunit -n BancaJornal.Tests
dotnet sln add BancaJornal.Tests

# Adicionar referências
dotnet add BancaJornal.Tests reference BancaJornal.Model
dotnet add BancaJornal.Tests reference BancaJornal.Application

# Executar testes
dotnet test
```

## ⚠️ Troubleshooting

### Erro de referência circular
- Verificar dependências em `.csproj`
- MODEL nunca deve referenciar outras camadas
- DESKTOP não deve referenciar REPOSITORY diretamente

### Erro de DI (serviço não registrado)
- Verificar `App.xaml.cs` → `ConfigureServices()`
- Ordem: DbContext → Repositories → Services → ViewModels

### Erro de migration
```powershell
# Deletar migration com problema
dotnet ef migrations remove --startup-project ../BancaJornal.Desktop

# Recriar banco do zero (cuidado: perde dados!)
rm bancajornal.db
dotnet ef database update --startup-project ../BancaJornal.Desktop
```

## 📚 Recursos Úteis

- [EF Core Docs](https://learn.microsoft.com/ef/core/)
- [WPF Docs](https://learn.microsoft.com/dotnet/desktop/wpf/)
- [MVVM Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Dica:** Sempre siga o fluxo MODEL → REPOSITORY → APPLICATION → DESKTOP ao adicionar funcionalidades.

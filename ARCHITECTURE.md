# Arquitetura do Sistema - Diagramas

## Diagrama de Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                     DESKTOP (Presentation)                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ MainWindow   │  │ DashboardView│  │ ProdutoView  │      │
│  │   (XAML)     │  │   (XAML)     │  │   (XAML)     │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                  │              │
│  ┌──────▼───────┐  ┌──────▼───────┐  ┌──────▼───────┐      │
│  │MainViewModel │  │Dashboard     │  │Produto       │      │
│  │              │  │ViewModel     │  │ViewModel     │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ Dependency Injection
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  APPLICATION (Use Cases)                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │Produto       │  │Venda         │  │Dashboard     │      │
│  │Service       │  │Service       │  │Service       │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                  │              │
│  ┌──────▼───────┐  ┌──────▼───────┐  ┌──────▼───────┐      │
│  │ProdutoDto    │  │VendaDto      │  │DashboardDto  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ Interfaces (DIP)
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  REPOSITORY (Data Access)                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │IUnitOfWork   │  │IProduto      │  │IVenda        │      │
│  │              │  │Repository    │  │Repository    │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                  │              │
│  ┌──────▼───────────────────────────────────▼───────┐      │
│  │           BancaJornalDbContext (EF Core)         │      │
│  └──────────────────────────────────────────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ Entity Framework
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      MODEL (Domain)                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │Produto       │  │Venda         │  │ItemVenda     │      │
│  │(Entity)      │  │(Aggregate)   │  │(Value Obj)   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │  SQLite DB    │
                    │bancajornal.db │
                    └───────────────┘
```

# Arquitetura do Sistema - Nova Versão

## Diagrama de Camadas

```
BancaJornal.Web (Blazor WebAssembly SPA)
      │
      │ IndexedDB/LocalStorage (Cache Offline)
      ▼
BancaJornal.Application (Services + DTOs)
      │
      ▼
BancaJornal.Repository (Interfaces + EF Core)
      │
      ▼
BancaJornal.Model (Domínio)
      │
      ▼
BancaJornal.Api (ASP.NET Core Web API)
      │
      ▼
SQLite DB (bancajornal.db)
```

## Fluxo de Dados

```
Usuário → Blazor SPA (View/Razor) → ViewModel → Service (Application)
→ [Cache Offline IndexedDB] → Repository → API → Banco SQLite

* Operações CRUD funcionam offline via IndexedDB
* Sincronização automática com backend quando disponível
```

## Padrões de Projeto Utilizados

| Padrão              | Onde                 | Propósito                           |
|---------------------|----------------------|-------------------------------------|
| **MVVM**            | Web (SPA)            | Separar UI de lógica                |
| **Repository**      | Repository           | Abstração de persistência           |
| **Unit of Work**    | Repository           | Gerenciar transações                |
| **Service Layer**   | Application          | Orquestrar casos de uso             |
| **DTO**             | Application          | Transferir dados entre camadas      |
| **Dependency Inj.** | Todo o sistema       | Desacoplamento e testabilidade      |
| **Aggregate Root**  | Model (Venda)        | Garantir consistência transacional  |
| **Value Object**    | Model (ItemVenda)    | Imutabilidade e ausência de ID      |

## Princípios SOLID Mapeados

## Fluxo de uma Venda (Exemplo Completo)

```
┌────────────┐
│   Usuário  │
└─────┬──────┘
      │ 1. Seleciona produto e quantidade
      ▼
┌─────────────────────┐
│   VendaView.xaml    │ (DESKTOP/View)
└─────────┬───────────┘
          │ 2. Binding XAML
          ▼
┌─────────────────────┐
│  VendaViewModel     │ (DESKTOP/ViewModel)
│  - AdicionarItem()  │
│  - FinalizarVenda() │
└─────────┬───────────┘
          │ 3. Command.Execute()
          ▼
┌─────────────────────┐
│   VendaService      │ (APPLICATION)
│  - CriarVendaAsync()│
└─────────┬───────────┘
          │ 4. Coordena operação
          ▼
┌─────────────────────┐
│   Venda (Entity)    │ (MODEL)
│  - AdicionarItem()  │
│  - Validações       │
└─────────┬───────────┘
          │ 5. Aplica regras de negócio
          ▼
┌─────────────────────┐
│  IVendaRepository   │ (REPOSITORY/Interface)
│  - AdicionarAsync() │
└─────────┬───────────┘
          │ 6. Implementação concreta
          ▼
┌─────────────────────┐
│  VendaRepository    │ (REPOSITORY/Impl)
│  + EF Core Context  │
└─────────┬───────────┘
          │ 7. SaveChangesAsync()
          ▼
┌─────────────────────┐
│   IUnitOfWork       │ (REPOSITORY)
│  - CommitAsync()    │
└─────────┬───────────┘
          │ 8. Transação
          ▼
      ┌───────┐
      │SQLite │
      └───────┘
```

## Princípios SOLID Mapeados

### Single Responsibility Principle (SRP)
```
✓ Produto.cs         → Gerencia apenas dados e regras de produto
✓ ProdutoRepository  → Apenas persistência de produto
✓ ProdutoService     → Apenas casos de uso de produto
✓ ProdutoViewModel   → Apenas lógica de apresentação de produto
```

### Open/Closed Principle (OCP)
```
✓ Produto.Atualizar()     → Métodos públicos para extensão
✓ Venda.AdicionarItem()   → Comportamento extensível
✓ Repository Interfaces   → Novas implementações sem modificar código
```

### Liskov Substitution Principle (LSP)
```
✓ IProdutoRepository → ProdutoRepository (substituível)
✓ IVendaRepository   → VendaRepository (substituível)
✓ IUnitOfWork        → UnitOfWork (substituível)
```

```
✓ IProdutoRepository  → Métodos específicos de Produto
✓ IVendaRepository    → Métodos específicos de Venda
✗ NÃO: IRepository<T> genérico com 50 métodos
```

### Dependency Inversion Principle (DIP)
```
✓ ProdutoService depende de IUnitOfWork (abstração)
✓ VendaViewModel depende de VendaService (abstração)
✓ App.xaml.cs configura DI container
✗ NÃO: new ProdutoRepository() direto no serviço
```

## Estrutura de Dependências

```
┌─────────────────────────────────────────┐
│         BancaJornal.Desktop             │
│         (WPF + MVVM + DI)               │
└────────────────┬────────────────────────┘
                 │ referencia
                 ▼
┌─────────────────────────────────────────┐
│      BancaJornal.Application            │
│         (Services + DTOs)               │
└──────────┬─────────────┬────────────────┘
           │             │
           │ referencia  │ referencia
           ▼             ▼
┌──────────────────┐  ┌─────────────────┐
│BancaJornal.Model │  │BancaJornal.     │
│  (Entities DDD)  │  │Repository       │
└──────────────────┘  │(EF + Interfaces)│
                      └─────┬───────────┘
                            │ referencia
                            ▼
                      ┌─────────────────┐
                      │BancaJornal.Model│
                      │  (Entities DDD) │
                      └─────────────────┘
```

## Padrões de Projeto Utilizados

| Padrão              | Onde                 | Propósito                           |
|---------------------|----------------------|-------------------------------------|
| **MVVM**            | Desktop              | Separar UI de lógica                |
| **Repository**      | Repository           | Abstração de persistência           |
| **Unit of Work**    | Repository           | Gerenciar transações                |
| **Service Layer**   | Application          | Orquestrar casos de uso             |
| **DTO**             | Application          | Transferir dados entre camadas      |
| **Dependency Inj.** | Todo o sistema       | Desacoplamento e testabilidade      |
| **Aggregate Root**  | Model (Venda)        | Garantir consistência transacional  |
| **Value Object**    | Model (ItemVenda)    | Imutabilidade e ausência de ID      |

## Checklist de Validação SOLID

Ao adicionar nova funcionalidade, verifique:

- [ ] Cada classe tem UMA responsabilidade clara?
- [ ] Posso estender sem modificar código existente?
- [ ] Implementações podem ser trocadas sem quebrar?
- [ ] Interfaces são específicas, não genéricas?
- [ ] Dependências são de abstrações, não de classes concretas?
- [ ] Validações estão no MODEL?
- [ ] ViewModel não acessa Repository diretamente?
- [ ] Serviços coordenam, mas não implementam regras de domínio?

---

**Para adicionar nova entidade:**
1. Criar entity em `MODEL`
2. Criar interface em `REPOSITORY/Interfaces`
3. Implementar repository em `REPOSITORY/Repositories`
4. Adicionar ao `IUnitOfWork`
5. Criar DTO em `APPLICATION/DTOs`
6. Criar Service em `APPLICATION/Services`
7. Criar ViewModel em `DESKTOP/ViewModels`
8. Criar View em `DESKTOP/Views`
9. Registrar DI em `App.xaml.cs`

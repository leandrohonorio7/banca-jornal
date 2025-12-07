using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blazored.LocalStorage;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

public partial class VendaViewModel : ObservableObject
{
    private readonly VendaService _vendaService;
    private readonly VendaIndexedDbService _vendaIndexedDbService;
    private readonly ILocalStorageService _localStorage;

    [ObservableProperty]
    private List<VendaDto> vendas;

    public VendaViewModel(VendaService vendaService, VendaIndexedDbService vendaIndexedDbService, ILocalStorageService localStorage)
    {
        _vendaService = vendaService;
        _vendaIndexedDbService = vendaIndexedDbService;
        _localStorage = localStorage;
        vendas = new List<VendaDto>();
    }

    [RelayCommand]
    public async Task CarregarVendasAsync()
    {
        // Tenta carregar do LocalStorage
        var cache = await _vendaIndexedDbService.ObterTodosAsync();
        if (cache != null && cache.Count > 0)
        {
            Vendas = cache;
        }
        else
        {
            Vendas = await _vendaService.ObterTodosAsync();
            await _vendaIndexedDbService.SalvarTodosAsync(Vendas);
            // Fallback: salva também no LocalStorage para compatibilidade
            await _localStorage.SetItemAsync("vendas", Vendas);
        }
    }
}

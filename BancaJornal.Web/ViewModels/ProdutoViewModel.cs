using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Blazored.LocalStorage;
using BancaJornal.Application.DTOs;
using BancaJornal.Application.Services;

public partial class ProdutoViewModel : ObservableObject
{
    private readonly ProdutoService _produtoService;
    private readonly ProdutoIndexedDbService _produtoIndexedDbService;
    private readonly ILocalStorageService _localStorage;

    [ObservableProperty]
    private List<ProdutoDto> produtos;

    public ProdutoViewModel(ProdutoService produtoService, ProdutoIndexedDbService produtoIndexedDbService, ILocalStorageService localStorage)
    {
        _produtoService = produtoService;
        _produtoIndexedDbService = produtoIndexedDbService;
        _localStorage = localStorage;
        produtos = new List<ProdutoDto>();
    }

    [RelayCommand]
    public async Task CarregarProdutosAsync()
    {
        // Tenta carregar do LocalStorage
        var cache = await _produtoIndexedDbService.ObterTodosAsync();
        if (cache != null && cache.Count > 0)
        {
            Produtos = cache;
        }
        else
        {
            // Busca do backend
            Produtos = await _produtoService.ObterTodosAsync();
            await _produtoIndexedDbService.SalvarTodosAsync(Produtos);
            // Fallback: salva também no LocalStorage para compatibilidade
            await _localStorage.SetItemAsync("produtos", Produtos);
        }
    }
}

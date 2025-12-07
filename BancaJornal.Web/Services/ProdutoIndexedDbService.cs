using Blazored.LocalStorage;
using BancaJornal.Application.DTOs;

public class ProdutoIndexedDbService
{
    private readonly ILocalStorageService _localStorage;
    public ProdutoIndexedDbService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    public async Task<List<ProdutoDto>> ObterTodosAsync()
    {
        var result = await _localStorage.GetItemAsync<List<ProdutoDto>>("produtos");
        return result ?? new List<ProdutoDto>();
    }
    public async Task SalvarTodosAsync(List<ProdutoDto> produtos)
    {
        await _localStorage.SetItemAsync("produtos", produtos);
    }
}

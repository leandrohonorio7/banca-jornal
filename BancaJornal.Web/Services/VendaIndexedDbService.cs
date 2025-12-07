using Blazored.LocalStorage;
using BancaJornal.Application.DTOs;

public class VendaIndexedDbService
{
    private readonly ILocalStorageService _localStorage;
    public VendaIndexedDbService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    public async Task<List<VendaDto>> ObterTodosAsync()
    {
        var result = await _localStorage.GetItemAsync<List<VendaDto>>("vendas");
        return result ?? new List<VendaDto>();
    }
    public async Task SalvarTodosAsync(List<VendaDto> vendas)
    {
        await _localStorage.SetItemAsync("vendas", vendas);
    }
}

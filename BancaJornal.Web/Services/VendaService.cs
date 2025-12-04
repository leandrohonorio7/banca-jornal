using System.Net.Http.Json;
using BancaJornal.Application.DTOs;

public class VendaService
{
    private readonly HttpClient _httpClient;
    public VendaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<VendaDto>> ObterTodosAsync()
    {
        var vendas = await _httpClient.GetFromJsonAsync<List<VendaDto>>("api/vendas");
        return vendas ?? new List<VendaDto>();
    }
}

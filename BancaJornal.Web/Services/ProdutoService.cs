using System.Net.Http.Json;
using BancaJornal.Application.DTOs;

public class ProdutoService
{
    private readonly HttpClient _httpClient;
    public ProdutoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ProdutoDto>> ObterTodosAsync()
    {
        var produtos = await _httpClient.GetFromJsonAsync<List<ProdutoDto>>("api/produtos");
        return produtos ?? new List<ProdutoDto>();
    }
}

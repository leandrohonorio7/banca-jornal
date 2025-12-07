using BancaJornal.Application.DTOs;

public class ProdutoSyncService
{
    private readonly ProdutoService _produtoService;
    private readonly ProdutoIndexedDbService _produtoIndexedDbService;

    public ProdutoSyncService(ProdutoService produtoService, ProdutoIndexedDbService produtoIndexedDbService)
    {
        _produtoService = produtoService;
        _produtoIndexedDbService = produtoIndexedDbService;
    }

    public async Task<List<ProdutoDto>> ObterProdutosAsync()
    {
        // Tenta buscar do backend
        try
        {
            var produtos = await _produtoService.ObterTodosAsync();
            await _produtoIndexedDbService.SalvarTodosAsync(produtos);
            return produtos;
        }
        catch
        {
            // Fallback para cache local
            return await _produtoIndexedDbService.ObterTodosAsync();
        }
    }

    public async Task SincronizarAsync()
    {
        var locais = await _produtoIndexedDbService.ObterTodosAsync();
        // TODO: Implementar lógica de sincronização quando houver método apropriado
        // await _produtoService.SalvarTodosAsync(locais);
    }
}

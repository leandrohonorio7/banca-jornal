using Blazor.IndexedDB.Framework;
using BancaJornal.Application.DTOs;

public class ProdutoIndexedDbService
{
    private readonly IndexedDBManager _dbManager;
    public ProdutoIndexedDbService(IndexedDBManager dbManager)
    {
        _dbManager = dbManager;
    }
    public async Task<List<ProdutoDto>> ObterTodosAsync()
    {
        var result = await _dbManager.GetRecords<ProdutoDto>("produtos");
        return result.ToList();
    }
    public async Task SalvarTodosAsync(List<ProdutoDto> produtos)
    {
        await _dbManager.ClearStore("produtos");
        foreach (var p in produtos)
            await _dbManager.AddRecord(new StoreRecord<ProdutoDto> { Storename = "produtos", Data = p });
    }
}

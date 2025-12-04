using Blazor.IndexedDB.Framework;
using BancaJornal.Application.DTOs;

public class VendaIndexedDbService
{
    private readonly IndexedDBManager _dbManager;
    public VendaIndexedDbService(IndexedDBManager dbManager)
    {
        _dbManager = dbManager;
    }
    public async Task<List<VendaDto>> ObterTodosAsync()
    {
        var result = await _dbManager.GetRecords<VendaDto>("vendas");
        return result.ToList();
    }
    public async Task SalvarTodosAsync(List<VendaDto> vendas)
    {
        await _dbManager.ClearStore("vendas");
        foreach (var v in vendas)
            await _dbManager.AddRecord(new StoreRecord<VendaDto> { Storename = "vendas", Data = v });
    }
}

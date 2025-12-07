using BancaJornal.Application.DTOs;

public class VendaSyncService
{
    private readonly VendaService _vendaService;
    private readonly VendaIndexedDbService _vendaIndexedDbService;

    public VendaSyncService(VendaService vendaService, VendaIndexedDbService vendaIndexedDbService)
    {
        _vendaService = vendaService;
        _vendaIndexedDbService = vendaIndexedDbService;
    }

    public async Task<List<VendaDto>> ObterVendasAsync()
    {
        try
        {
            var vendas = await _vendaService.ObterTodosAsync();
            await _vendaIndexedDbService.SalvarTodosAsync(vendas);
            return vendas;
        }
        catch
        {
            return await _vendaIndexedDbService.ObterTodosAsync();
        }
    }

    public async Task SincronizarAsync()
    {
        var locais = await _vendaIndexedDbService.ObterTodosAsync();
        // TODO: Implementar lógica de sincronização quando houver método apropriado
        // await _vendaService.SalvarTodosAsync(locais);
    }
}

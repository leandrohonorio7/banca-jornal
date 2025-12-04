using Microsoft.AspNetCore.Mvc;
using BancaJornal.Application.Services;
using BancaJornal.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class VendaController : ControllerBase
{
    private readonly VendaService _vendaService;
    public VendaController(VendaService vendaService)
    {
        _vendaService = vendaService;
    }
    [HttpGet]
    public async Task<ActionResult<List<VendaDto>>> Get()
    {
        var vendas = await _vendaService.ObterTodosAsync();
        return Ok(vendas);
    }
}

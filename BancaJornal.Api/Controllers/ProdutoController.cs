using Microsoft.AspNetCore.Mvc;
using BancaJornal.Application.Services;
using BancaJornal.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly ProdutoService _produtoService;
    public ProdutoController(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }
    [HttpGet]
    public async Task<ActionResult<List<ProdutoDto>>> Get()
    {
        var produtos = await _produtoService.ObterTodosAsync();
        return Ok(produtos);
    }
}

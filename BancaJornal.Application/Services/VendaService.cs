using BancaJornal.Application.DTOs;
using BancaJornal.Model.Entities;
using BancaJornal.Repository.Interfaces;

namespace BancaJornal.Application.Services;

/// <summary>
/// Serviço de aplicação para gerenciamento de vendas.
/// Coordena operações de venda e atualização de estoque.
/// Aplica SRP - responsabilidade única de orquestrar vendas.
/// </summary>
public class VendaService
{
    private readonly IUnitOfWork _unitOfWork;

    public VendaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<VendaDto?> ObterPorIdAsync(int id)
    {
        var venda = await _unitOfWork.Vendas.ObterPorIdAsync(id);
        return venda != null ? VendaDto.FromEntity(venda) : null;
    }

    public async Task<IEnumerable<VendaDto>> ObterTodosAsync()
    {
        var vendas = await _unitOfWork.Vendas.ObterTodosAsync();
        return vendas.Select(VendaDto.FromEntity);
    }

    public async Task<IEnumerable<VendaDto>> ObterVendasDoMesAsync(int mes, int ano)
    {
        var vendas = await _unitOfWork.Vendas.ObterVendasDoMesAsync(mes, ano);
        return vendas.Select(VendaDto.FromEntity);
    }

    /// <summary>
    /// Cria uma nova venda e atualiza o estoque dos produtos vendidos.
    /// Garante transação atômica usando Unit of Work.
    /// </summary>
    public async Task<VendaDto> CriarVendaAsync(List<(int ProdutoId, int Quantidade)> itens, string? observacao = null)
    {
        if (itens == null || !itens.Any())
            throw new ArgumentException("A venda deve conter pelo menos um item.", nameof(itens));

        try
        {
            var venda = new Venda(observacao);

            // Adicionar itens à venda e atualizar estoque
            foreach (var (produtoId, quantidade) in itens)
            {
                var produto = await _unitOfWork.Produtos.ObterPorIdAsync(produtoId);
                if (produto == null)
                    throw new InvalidOperationException($"Produto com ID {produtoId} não encontrado.");

                // Adicionar item à venda
                venda.AdicionarItem(produto, quantidade);

                // Atualizar estoque (permite estoque negativo para controle)
                produto.RemoverEstoque(quantidade);
                await _unitOfWork.Produtos.AtualizarAsync(produto);
            }

            if (!venda.PodeSerFinalizada())
                throw new InvalidOperationException("A venda não pode ser finalizada sem itens válidos.");

            await _unitOfWork.Vendas.AdicionarAsync(venda);
            await _unitOfWork.CommitAsync();

            return VendaDto.FromEntity(venda);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<decimal> ObterTotalVendasMesAsync(int mes, int ano)
    {
        return await _unitOfWork.Vendas.ObterTotalVendasMesAsync(mes, ano);
    }

    public async Task<int> ContarVendasMesAsync(int mes, int ano)
    {
        return await _unitOfWork.Vendas.ContarVendasMesAsync(mes, ano);
    }
}

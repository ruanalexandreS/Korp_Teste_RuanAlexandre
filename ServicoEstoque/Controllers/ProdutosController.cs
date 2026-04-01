using Microsoft.EntityFrameworkCore;
using ServicoEstoque.Data;
using ServicoEstoque.Models;
using Microsoft.AspNetCore.Mvc;

namespace ServicoEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueContext _context;

        // SemaphoreSlim estático garante que apenas uma thread
        // por vez execute a baixa de estoque (controle de concorrência)
        private static readonly SemaphoreSlim _semaforo = new SemaphoreSlim(1, 1);

        public ProdutosController(EstoqueContext context)
        {
            _context = context;
        }

        // Lista todos os produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        // Cadastra um novo produto
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        // Busca um produto pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            return produto;
        }

        // Atualiza o saldo com controle de concorrência
        [HttpPut("{id}/atualizar-saldo")]
        public async Task<IActionResult> AtualizarSaldo(int id, [FromBody] int quantidadeUtilizada)
        {
            // Bloqueia outras requisições simultâneas
            await _semaforo.WaitAsync();
            try
            {
                // Relê o produto do banco dentro do lock para garantir saldo atual
                var produto = await _context.Produtos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (produto == null)
                    return NotFound("Produto não encontrado.");

                if (produto.Saldo < quantidadeUtilizada)
                    return BadRequest($"Saldo insuficiente. Disponível: {produto.Saldo}");

                // Usa ExecuteUpdateAsync para atualizar direto no banco
                // evitando race condition entre leitura e escrita
                await _context.Produtos
                    .Where(p => p.Id == id && p.Saldo >= quantidadeUtilizada)
                    .ExecuteUpdateAsync(s => s.SetProperty(
                        p => p.Saldo, p => p.Saldo - quantidadeUtilizada));

                return NoContent();
            }
            finally
            {
                // Libera o semáforo mesmo se der erro
                _semaforo.Release();
            }
        }
    }
}
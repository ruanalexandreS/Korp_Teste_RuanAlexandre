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

        //Injenção de Dependência
        public ProdutosController(EstoqueContext context)
        {
            _context = context;
        }

        // Lista todos os produtos (Requisito do PDF)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            //LINQ: _context.Produtos.ToListAsync()
            return await _context.Produtos.ToListAsync();
        }

        // Cadastra um novo produto (Requisito do PDF)
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Retorna um status 201 (Created)
            // e aponta para o endpoint "GetProduto" para encontrar o novo item
            return CreatedAtAction(nameof(GetProdutos), new { id = produto.Id }, produto);
        }

        // Busca um produto pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound(); // Retorna 404 se não achar
            }

            return produto;
        }

        // Atualiza o saldo (Requisito do PDF para a nota fiscal)
        [HttpPut("{id}/atualizar-saldo")]
        public async Task<IActionResult> AtualizarSaldo(int id, [FromBody] int quantidadeUtilizada)
        {
             var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            if (produto.Saldo < quantidadeUtilizada)
            {
                return BadRequest("Saldo insuficiente."); // Retorna 400 
            }

            produto.Saldo -= quantidadeUtilizada; //Atualiza o saldo

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 (Sucesso, sem conteúdo)
        }
    }
}

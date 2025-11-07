using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicoFaturamento.Data;
using ServicoFaturamento.Models;

namespace ServicoFaturamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotasFiscaisController : ControllerBase
    {
        private readonly FaturamentoContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotasFiscaisController> _logger;
        private const string StatusAberta = "Aberta";
        private const string StatusFechada = "Fechada";

        public NotasFiscaisController(FaturamentoContext context, IHttpClientFactory httpClientFactory, ILogger<NotasFiscaisController> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // POST: api/notasfiscais
        // Cadastra uma nova Nota Fiscal (Requisito do PDF)
        [HttpPost]
        public async Task<ActionResult<NotaFiscal>> PostNotaFiscal([FromBody] NotaFiscal notaFiscal)
        {
            if (notaFiscal == null)
                return BadRequest("Nota fiscal obrigatória.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            notaFiscal.Status = StatusAberta;

            const int maxAttempts = 3;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var ultimaNota = await _context.NotasFiscais.OrderByDescending(n => n.NumeroSequencial).FirstOrDefaultAsync();
                    notaFiscal.NumeroSequencial = (ultimaNota?.NumeroSequencial ?? 0) + 1;

                    _context.NotasFiscais.Add(notaFiscal);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetNotaFiscal), new { id = notaFiscal.Id }, notaFiscal);
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning(ex, "Tentativa {Attempt} para inserir nota falhou por DbUpdateException.", attempt);
                    if (attempt == maxAttempts)
                    {
                        _logger.LogError(ex, "Falha ao inserir nota fiscal após {MaxAttempts} tentativas.", maxAttempts);
                        return StatusCode(500, "Erro ao salvar a nota fiscal.");
                    }
                    await Task.Delay(50 * attempt);
                }
            }

            return StatusCode(500, "Não foi possível criar a nota fiscal.");
        }

        // Busca uma nota fiscal pelo ID (para usarmos no CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<ActionResult<NotaFiscal>> GetNotaFiscal(int id)
        {
            var notaFiscal = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);

            if (notaFiscal == null)
            {
                return NotFound();
            }

            return notaFiscal;
        }

        // GET: api/notasfiscais/listar
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> ListarNotasFiscais(
            [FromQuery] int pagina = 1,
            [FromQuery] int itensPorPagina = 10)
        {
            var query = _context.NotasFiscais.AsNoTracking().Include(n => n.Itens).OrderByDescending(n => n.NumeroSequencial);

            // Calcula total de registros para paginação
            var total = await query.CountAsync();

            // Aplica paginação
            var notas = await query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();

            // Adiciona header com total de registros (útil para o frontend)
            Response.Headers.Add("X-Total-Count", total.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
            return Ok(notas);
        }

        // POST: api/notasfiscais/{id}/imprimir
        [HttpPost("{id}/imprimir")]
        public async Task<IActionResult> ImprimirNotaFiscal(int id)
        {
            var notaFiscal = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);

            if (notaFiscal == null) return NotFound("Nota Fiscal não encontrada.");

            if (notaFiscal.Status != StatusAberta)
            {
                return BadRequest($"Esta nota fiscal não pode ser impressa (Status: {notaFiscal.Status}).");
            }

            var httpClient = _httpClientFactory.CreateClient("ServicoEstoque");

            try
            {
                if (notaFiscal.Itens != null)
                {
                    foreach (var item in notaFiscal.Itens)
                    {
                        // Chama a API: POST https://.../api/produtos/{id}/atualizar-saldo
                        var response = await httpClient.PutAsJsonAsync($"api/produtos/{item.ProdutoId}/atualizar-saldo", item.Quantidade);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Falha ao atualizar o estoque para a nota {NotaId}.", id);
                // 502 Bad Gateway indica falha em serviço dependente
                return StatusCode(502, $"Falha ao atualizar o estoque: {ex.Message}");
            }

            notaFiscal.Status = StatusFechada;
            _context.Entry(notaFiscal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status da nota {NotaId} para Fechada.", id);
                return StatusCode(500, "Erro ao atualizar o status da nota fiscal.");
            }

            return Ok("Nota impressa com sucesso e estoque atualizado.");
        }
    }
}

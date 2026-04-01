using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ServicoFaturamento.Controllers;
using ServicoFaturamento.Data;
using ServicoFaturamento.Models;

namespace ServicoFaturamento.Tests;

public class NotasFiscaisControllerTests
{
    private FaturamentoContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<FaturamentoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FaturamentoContext(options);
    }

    private NotasFiscaisController CriarController(FaturamentoContext context, HttpClient? httpClient = null)
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        var mockLogger = new Mock<ILogger<NotasFiscaisController>>();

        if (httpClient != null)
            mockFactory.Setup(f => f.CreateClient("ServicoEstoque")).Returns(httpClient);

        return new NotasFiscaisController(context, mockFactory.Object, mockLogger.Object);
    }

    // TESTE 1: Nota criada deve ter status "Aberta"
    [Fact]
    public async Task PostNotaFiscal_DeveCriarComStatusAberta()
    {
        var context = CriarContexto();
        var controller = CriarController(context);

        var nota = new NotaFiscal
        {
            Itens = new List<ItemNotaFiscal>
            {
                new ItemNotaFiscal { ProdutoId = 1, Quantidade = 2 }
            }
        };

        var resultado = await controller.PostNotaFiscal(nota);

        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        var notaCriada = Assert.IsType<NotaFiscal>(createdResult.Value);
        Assert.Equal("Aberta", notaCriada.Status);
    }

    // TESTE 2: Numeração sequencial deve incrementar
    [Fact]
    public async Task PostNotaFiscal_DeveIncrementarNumeroSequencial()
    {
        var context = CriarContexto();
        var controller = CriarController(context);

        var nota1 = new NotaFiscal { Itens = new List<ItemNotaFiscal> { new() { ProdutoId = 1, Quantidade = 1 } } };
        var nota2 = new NotaFiscal { Itens = new List<ItemNotaFiscal> { new() { ProdutoId = 1, Quantidade = 1 } } };

        await controller.PostNotaFiscal(nota1);
        await controller.PostNotaFiscal(nota2);

        Assert.Equal(1, nota1.NumeroSequencial);
        Assert.Equal(2, nota2.NumeroSequencial);
    }

    // TESTE 3: Não deve imprimir nota já fechada
    [Fact]
    public async Task ImprimirNota_QuandoJaFechada_DeveRetornarBadRequest()
    {
        var context = CriarContexto();
        var nota = new NotaFiscal
        {
            NumeroSequencial = 1,
            Status = "Fechada",
            Itens = new List<ItemNotaFiscal>()
        };
        context.NotasFiscais.Add(nota);
        await context.SaveChangesAsync();

        var controller = CriarController(context);

        var resultado = await controller.ImprimirNotaFiscal(nota.Id, null);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    // TESTE 4: Deve retornar NotFound para nota inexistente
    [Fact]
    public async Task ImprimirNota_QuandoNaoExiste_DeveRetornarNotFound()
    {
        var context = CriarContexto();
        var controller = CriarController(context);

        var resultado = await controller.ImprimirNotaFiscal(999, null);

        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    // TESTE 5: Idempotência — requisição repetida deve retornar resultado cacheado
    [Fact]
    public async Task ImprimirNota_ComChaveRepetida_DeveRetornarResultadoCacheado()
    {
        var context = CriarContexto();
        var chave = "chave-unica-123";
        var resultadoEsperado = "Nota 1 impressa com sucesso e estoque atualizado.";

        context.RequisicoesProcessadas.Add(new RequisicaoProcessada
        {
            IdempotencyKey = chave,
            ProcessadoEm = DateTime.UtcNow,
            Resultado = resultadoEsperado
        });
        await context.SaveChangesAsync();

        var controller = CriarController(context);
        var resultado = await controller.ImprimirNotaFiscal(1, chave);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(resultadoEsperado, okResult.Value);
    }
}
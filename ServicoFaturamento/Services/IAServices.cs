using System.Text;
using System.Text.Json;
using ServicoFaturamento.Models;

namespace ServicoFaturamento.Services;

public class IAService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public IAService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Anthropic:ApiKey"]!;
    }

    public async Task<string> GerarResumoNotaAsync(NotaFiscal nota)
    {
        var itens = string.Join(", ", nota.Itens.Select(i =>
            $"{i.Quantidade}x Produto #{i.ProdutoId}"));

        var prompt = $"""
            Você é um sistema de ERP. Gere um resumo profissional e conciso 
            (máximo 2 frases) para esta nota fiscal:
            - Número: {nota.NumeroSequencial}
            - Itens: {itens}
            - Total de itens: {nota.Itens.Sum(i => i.Quantidade)}
            Responda apenas com o resumo, sem explicações adicionais.
            """;

        return await ChamarClaudeAsync(prompt);
    }

    private async Task<string> ChamarClaudeAsync(string prompt)
    {
        var payload = new
        {
            model = "claude-haiku-4-5-20251001",
            max_tokens = 200,
            messages = new[] { new { role = "user", content = prompt } }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? "Resumo não disponível.";
    }
}
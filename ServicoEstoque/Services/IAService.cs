using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ServicoEstoque.Models;

namespace ServicoEstoque.Services;

public class IAService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public IAService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Anthropic:ApiKey"]!;
    }

    public async Task<string> GerarSugestaoEstoqueAsync(List<Produto> produtos)
    {
        var lista = string.Join("\n", produtos.Select(p =>
            $"- {p.Descricao} (Código: {p.Codigo}, Saldo: {p.Saldo})"));

        var prompt = $"""
            Você é um assistente de gestão de estoque. Analise os produtos abaixo
            e identifique quais precisam de reposição urgente (saldo abaixo de 5)
            e quais estão com estoque adequado. Seja objetivo e profissional.
            
            Produtos:
            {lista}
            
            Responda em no máximo 3 frases com as recomendações principais.
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
            .GetString() ?? "Sugestão não disponível.";
    }
}
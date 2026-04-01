namespace ServicoFaturamento.Models;

public class RequisicaoProcessada
{
    public int Id { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTime ProcessadoEm { get; set; }
    public string Resultado { get; set; } = string.Empty;
}
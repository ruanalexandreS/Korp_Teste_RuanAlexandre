using System.ComponentModel.DataAnnotations;

namespace ServicoFaturamento.Models
{
    public class NotaFiscal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numeração sequencial é obrigatória")]
        public int NumeroSequencial { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } = "Aberta";

        [Required(ErrorMessage = "A nota fiscal precisa ter pelo menos um item")]
        public List<ItemNotaFiscal> Itens { get; set; } = new();
    }
}
using System.ComponentModel.DataAnnotations;

namespace ServicoFaturamento.Models
{
    public class ItemNotaFiscal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Código do produto é obrigatório")]
        public int ProdutoId { get; set; } 

        [Required(ErrorMessage = "A Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        public int NotaFiscalId { get; set; }
        public NotaFiscal? NotaFiscal { get; set; }
    }
}
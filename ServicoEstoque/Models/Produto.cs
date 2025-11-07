using System.ComponentModel.DataAnnotations;

namespace ServicoEstoque.Models 
{
    public class  Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O código é obrigatório")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Saldo deve ser maior ou igual a zero")]
        public int Saldo { get; set; }
    }
}
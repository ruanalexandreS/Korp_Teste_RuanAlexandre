using System.ComponentModel.DataAnnotations;

namespace ServicoEstoque.Models 
{
    public class  Produto
    {
        [Key]
        public int Id { get; set; }

        [cite_start]
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int Saldo { get; set; }
    }
}
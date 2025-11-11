using Microsoft.EntityFrameworkCore;
using ServicoFaturamento.Models;

namespace ServicoFaturamento.Data
{
    public class FaturamentoContext : DbContext
    {
        public FaturamentoContext(DbContextOptions<FaturamentoContext> options) : base(options)
        {
        }

        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<ItemNotaFiscal> ItensNotaFiscal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da NotaFiscal
            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                //Indice único para o campo NumeroSequencial
                entity.HasIndex(e => e.NumeroSequencial).IsUnique();

                //Indice para Status (ajuda em consultas por status)
                entity.HasIndex(e => e.Status);

                // Configura o Status como required e com tamanho máximo
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                entity.HasMany(e => e.Itens)
                      .WithOne()
                      .HasForeignKey(e => e.NotaFiscalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
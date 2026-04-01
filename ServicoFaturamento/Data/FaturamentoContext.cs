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
        public DbSet<RequisicaoProcessada> RequisicoesProcessadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.HasIndex(e => e.NumeroSequencial).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.HasMany(e => e.Itens)
                      .WithOne()
                      .HasForeignKey(e => e.NotaFiscalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Índice único para evitar duplicatas de idempotência
            modelBuilder.Entity<RequisicaoProcessada>(entity =>
            {
                entity.HasIndex(e => e.IdempotencyKey).IsUnique();
                entity.Property(e => e.IdempotencyKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Resultado).IsRequired().HasMaxLength(500);
            });
        }
    }
}
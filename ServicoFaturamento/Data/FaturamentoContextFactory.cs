using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServicoFaturamento.Data;

public class FaturamentoContextFactory : IDesignTimeDbContextFactory<FaturamentoContext>
{
    public FaturamentoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FaturamentoContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=KorpFaturamento;User Id=sa;Password=KorpSenha123!;TrustServerCertificate=True;");
        return new FaturamentoContext(optionsBuilder.Options);
    }
}
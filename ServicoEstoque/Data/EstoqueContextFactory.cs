using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServicoEstoque.Data;

public class EstoqueContextFactory : IDesignTimeDbContextFactory<EstoqueContext>
{
    public EstoqueContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EstoqueContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=KorpEstoque;User Id=sa;Password=KorpSenha123!;TrustServerCertificate=True;");
        return new EstoqueContext(optionsBuilder.Options);
    }
}
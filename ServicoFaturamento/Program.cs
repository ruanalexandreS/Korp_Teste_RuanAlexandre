using Microsoft.EntityFrameworkCore;
using ServicoFaturamento.Data;
using ServicoFaturamento.Models;
using System.Data.SqlTypes;
using ServicoFaturamento.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Configuração do banco de dados
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDb)
{
    // Configuração para banco em memória (desenvolvimento/testes)
    builder.Services.AddDbContext<FaturamentoContext>(options =>
    options.UseInMemoryDatabase("Faturamento"));
}
else
{
    // Configuração para SQL Server (produção)
    builder.Services.AddDbContext<FaturamentoContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorNumbersToAdd: null)));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do HttpClient para o ServiçoEstoque
builder.Services.AddHttpClient("ServicoEstoque", client =>
{
    client.BaseAddress = new Uri("https://localhost:7296");
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
      app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/", () => 
$"Serviço de Faturamento está rodando... Usando banco: {(useInMemoryDb ? "Em Memória" : "SQL Server")}");
app.Run();
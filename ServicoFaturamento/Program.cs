using Microsoft.EntityFrameworkCore;
using ServicoFaturamento.Data;
using ServicoFaturamento.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Configura��o do banco de dados
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDb)
{
    // Configura��o para banco em mem�ria (desenvolvimento/testes)
    builder.Services.AddDbContext<FaturamentoContext>(options =>
    options.UseInMemoryDatabase("Faturamento"));
}
else
{
    // Configura��o para SQL Server (produ��o)
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

// Configura��o do HttpClient para o Servi�oEstoque
builder.Services.AddHttpClient("ServicoEstoque", client =>
{
    client.BaseAddress = new Uri("https://localhost:7296");
});

builder.Services.AddHttpClient<ServicoFaturamento.Services.IAService>();

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
$"Servi�o de Faturamento est� rodando... Usando banco: {(useInMemoryDb ? "Em Mem�ria" : "SQL Server")}");
app.Run();
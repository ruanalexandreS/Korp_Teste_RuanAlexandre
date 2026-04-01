using Microsoft.EntityFrameworkCore;
using ServicoEstoque.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// ZONA 1: CONFIGURAÇÃO DE SERVIÇOS (DI)
// ==========================================

builder.Services.AddControllers();

// Configuração do Banco de Dados
var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
if (useInMemory)
{
    builder.Services.AddDbContext<EstoqueContext>(opt =>
        opt.UseInMemoryDatabase("Estoque"));
}
else
{
    builder.Services.AddDbContext<EstoqueContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Registro de HttpClient para serviços externos
builder.Services.AddHttpClient<ServicoEstoque.Services.IAService>();

// ==========================================
// LINHA DIVISÓRIA: CONSTRUÇÃO DO APP
// ==========================================
var app = builder.Build();

// ==========================================
// ZONA 2: PIPELINE DE MIDDLEWARES
// ==========================================

// 1. SEGURANÇA (API KEY)
// Deve ser o primeiro para bloquear acessos antes de processar qualquer lógica
app.Use(async (context, next) =>
{
    // Deixa o Swagger passar sem precisar de key (para testes em dev)
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    // Verifica se o header X-Internal-Key existe e está correto
    var keyEsperada = builder.Configuration["InternalApiKey"];

    if (!context.Request.Headers.TryGetValue("X-Internal-Key", out var keyRecebida)
        || keyRecebida != keyEsperada)
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Acesso não autorizado: Chave de API inválida ou ausente.");
        return;
    }

    // Key correta → prossegue para o próximo middleware
    await next();
});

// 2. DOCUMENTAÇÃO (SWAGGER)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. POLÍTICAS DE TRÁFEGO
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// 4. MAPEAMENTO DE ROTAS (ENDPOINTS)
app.MapControllers();
app.MapGet("/", () => "API de Estoque está rodando e protegida!");

// 5. INICIALIZAÇÃO
app.Run();
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServicoFaturamento.Auth;
using ServicoFaturamento.Data;
using ServicoFaturamento.Middleware;
using ServicoFaturamento.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Configurações de Log e Controladores ---
builder.Services.AddControllers();
builder.Services.AddLogging();

// --- 2. Configurações de JWT (Segurança) ---
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<TokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

// --- 3. Configuração do Banco de Dados ---
var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDb)
{
    builder.Services.AddDbContext<FaturamentoContext>(options =>
        options.UseInMemoryDatabase("Faturamento"));
}
else
{
    builder.Services.AddDbContext<FaturamentoContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null)));
}

// --- 4. CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://korp-teste-ruan-alexandre.vercel.app/"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// --- 5. Swagger com Suporte a JWT ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Serviço de Faturamento", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no campo abaixo. Exemplo: '12345abcdef'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- 6. Clientes HTTP e Serviços Customizados ---
builder.Services.AddHttpClient("ServicoEstoque", client =>
{
    var url = builder.Configuration["ServicosUrl:Estoque"]
            ?? "https://localhost:7296";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient<IAService>();

// --- BUILD ---
var app = builder.Build();

// --- 7. Pipeline de Middleware (A ORDEM IMPORTA) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1º Tratamento de Erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2º Segurança e Redirecionamento
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// 3º Autenticação e Autorização (Sempre antes dos Controllers)
app.UseAuthentication(); 
app.UseAuthorization();

// 4º Mapeamento de Rotas
app.MapControllers();

app.MapGet("/", () => 
    $"Serviço de Faturamento está rodando... Usando banco: {(useInMemoryDb ? "Em Memória" : "SQL Server")}");

app.Run();
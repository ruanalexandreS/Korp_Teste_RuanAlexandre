using Microsoft.EntityFrameworkCore;
using ServicoEstoque.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddHttpClient<ServicoEstoque.Services.IAService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => "API de Estoque está rodando!");

app.Run();
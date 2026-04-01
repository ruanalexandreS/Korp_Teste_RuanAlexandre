# Teste Prático Korp - Desenvolvedor Jr. (C# + Angular)

Este repositório contém a solução completa para o desafio técnico da Korp, implementando um sistema de emissão de notas fiscais com arquitetura de microsserviços em .NET e frontend em Angular.

---

## 🚀 Funcionalidades Implementadas

### Requisitos Obrigatórios

* ✅ **Arquitetura de Microsserviços:** Backend dividido em dois serviços independentes: `ServicoEstoque` e `ServicoFaturamento`.
* ✅ **Cadastro de Produtos:** Frontend permite cadastro de produtos (Código, Descrição, Saldo) persistidos no `ServicoEstoque`.
* ✅ **Cadastro de Notas Fiscais:** Frontend permite criação de notas fiscais com um ou mais itens.
* ✅ **Impressão de Notas Fiscais:** Ao imprimir, o `ServicoFaturamento` chama o `ServicoEstoque` para dar baixa no saldo e atualiza o status para "Fechada".
* ✅ **Tratamento de Falhas:** Se o `ServicoEstoque` estiver offline, o erro é reportado, a nota permanece "Aberta" e o frontend exibe notificação de falha.

### ✨ Funcionalidades Adicionais (Bônus)

* ✅ **Idempotência:** O endpoint de impressão aceita um `Idempotency-Key` no header. Requisições repetidas com a mesma chave retornam o resultado cacheado sem reprocessar.
* ✅ **Controle de Concorrência:** O `ServicoEstoque` usa `SemaphoreSlim` + `ExecuteUpdateAsync` para garantir que duas impressões simultâneas do mesmo produto não causem inconsistência de saldo.
* ✅ **Testes Unitários:** 5 testes no projeto `ServicoFaturamento.Tests` cobrindo criação de notas, numeração sequencial, validação de status, idempotência e tratamento de erros.
* ✅ **Inteligência Artificial:** Integração com a API da Anthropic (Claude) em dois pontos:
  * Ao imprimir uma nota, a IA gera um resumo profissional automaticamente.
  * Endpoint `GET /api/produtos/sugestao-estoque` analisa o estoque e sugere reposições.
* ✅ **SQL Server:** Ambos os serviços alternam entre `InMemoryDatabase` e `SQL Server` via flag `UseInMemoryDatabase` no `appsettings.json`.
* ✅ **Reatividade em Tempo Real:** Listas se atualizam sem F5 usando `Subject` do RxJS.
* ✅ **Middleware de Erros:** `ErrorHandlingMiddleware` global no `ServicoFaturamento` captura exceções e retorna respostas JSON padronizadas.
* ✅ **Paginação no Backend:** `GET /listar` usa `Skip`/`Take` para paginação.

---

## 🛠️ Arquitetura e Tecnologias

### Estrutura da Solução
```
Korp_Teste_RuanAlexandre/
├── ServicoEstoque/          # API REST - Controle de produtos e saldos
├── ServicoFaturamento/      # API REST - Gestão de notas fiscais
├── ServicoFaturamento.Tests/ # Testes unitários (xUnit + Moq)
└── KorpApp/                 # Frontend Angular
```

### Stack

| Camada | Tecnologia |
|--------|-----------|
| Backend | .NET 9, ASP.NET Core, EF Core 9 |
| Banco de Dados | SQL Server (Podman/Docker) ou InMemory |
| Frontend | Angular 17+, Angular Material, RxJS |
| Testes | xUnit, Moq, EF InMemory |
| IA | Anthropic Claude API (claude-haiku) |

---

## 🧠 Decisões Técnicas

### Por que `IHttpClientFactory` e não `HttpClient` diretamente?
O `HttpClient` instanciado manualmente sofre de *socket exhaustion* em aplicações de longa duração. O `IHttpClientFactory` gerencia o ciclo de vida dos `HttpMessageHandler`, reutilizando conexões TCP de forma eficiente e segura.

### Por que `SemaphoreSlim` para controle de concorrência?
É thread-safe, async-friendly e não bloqueia a thread pool. Garante que apenas uma thread por vez execute a baixa de estoque, evitando race conditions sem precisar de transações distribuídas complexas. Combinado com `ExecuteUpdateAsync`, a atualização é feita diretamente no banco em uma única operação atômica.

### Por que Idempotency Key no header?
Padrão de mercado adotado por Stripe e AWS. Permite que o cliente reenvie a requisição com segurança em caso de timeout ou falha de rede, sem causar efeitos colaterais como cobranças ou baixas de estoque duplicadas.

### Por que SQL Server via Podman e não instalação nativa?
No Linux (Bazzite/Fedora), o SQL Server não tem instalação nativa. Podman é o equivalente rootless do Docker, já incluído no Bazzite, e permite rodar o SQL Server em container sem configurações complexas.

### LINQ utilizado
* `OrderByDescending` + `FirstOrDefaultAsync` — numeração sequencial de notas
* `Skip` / `Take` — paginação na listagem de notas
* `Sum` — total de itens para o resumo da IA
* `Select` — projeção de dados para o prompt da IA
* `Where` + `ExecuteUpdateAsync` — atualização atômica do saldo

### Ciclos de vida do Angular utilizados
* `ngOnInit` — carregamento inicial de produtos e notas via `HttpClient`
* `OnDestroy` + `unsubscribe` — prevenção de memory leaks nos observables

### RxJS
* `Subject` — notifica componentes irmãos após cadastro sem necessidade de `EventEmitter`
* `subscribe` — reage às respostas das APIs REST

---

## ⚙️ Configuração do Banco de Dados

### Rodando o SQL Server com Podman
```bash
podman run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=KorpSenha123!" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Configurando o `appsettings.json`

> ⚠️ O arquivo `appsettings.json` **não está no repositório** por conter credenciais. Crie-o manualmente em cada serviço:

**ServicoEstoque/appsettings.json:**
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=KorpEstoque;User Id=sa;Password=KorpSenha123!;TrustServerCertificate=True;"
  },
  "Anthropic": {
    "ApiKey": "SUA_CHAVE_AQUI"
  }
}
```

**ServicoFaturamento/appsettings.json:**
```json
{
  "UseInMemoryDatabase": false,
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=KorpFaturamento;User Id=sa;Password=KorpSenha123!;TrustServerCertificate=True;"
  },
  "Anthropic": {
    "ApiKey": "SUA_CHAVE_AQUI"
  }
}
```

### Aplicando as migrations
```bash
cd ServicoEstoque
dotnet ef database update

cd ../ServicoFaturamento
dotnet ef database update
```

---

## ▶️ Como Executar o Projeto

### Pré-requisitos

* .NET 9 SDK
* Node.js (LTS) + Angular CLI (`npm install -g @angular/cli`)
* Podman ou Docker (para o SQL Server)
* VS Code

### 1. Rodando o Backend
```bash
# Terminal 1 - ServicoEstoque
cd ServicoEstoque
dotnet run

# Terminal 2 - ServicoFaturamento
cd ServicoFaturamento
dotnet run
```

### 2. Rodando o Frontend
```bash
cd KorpApp
npm install
ng serve -o
```

Acesse `http://localhost:4200`.

> **Nota:** Verifique as URLs nos serviços Angular caso as portas sejam diferentes:
> * `KorpApp/src/app/services/estoque.service.ts`
> * `KorpApp/src/app/services/faturamento.service.ts`

---

## 🧪 Rodando os Testes
```bash
cd ServicoFaturamento.Tests
dotnet test
```

**Resultado esperado:** 5 testes, 0 falhas.

---

## 🧪 Como Testar os Requisitos

### Fluxo de Sucesso

1. Cadastre um produto (ex: "Produto A", Saldo "20")
2. Crie uma nota fiscal com esse produto (Quantidade: 5)
3. Clique em "Imprimir"
4. **Resultado:** Status muda para "Fechada", saldo reduz para 15, e a IA exibe um resumo da nota

### Fluxo de Falha

1. Cadastre um produto e uma nota
2. Pare o `ServicoEstoque`
3. Clique em "Imprimir"
4. **Resultado:** Notificação de erro, nota permanece "Aberta"

### Sugestão de Estoque via IA
```
GET https://localhost:7296/api/produtos/sugestao-estoque
```

Retorna análise inteligente dos produtos com recomendações de reposição.
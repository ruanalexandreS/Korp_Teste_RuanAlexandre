# 🧾 Korp — Sistema de Emissão de Notas Fiscais

<div align="center">

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Railway](https://img.shields.io/badge/Railway-0B0D0E?style=for-the-badge&logo=railway&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

![Build](https://img.shields.io/github/actions/workflow/status/ruanalexandreS/Korp_Teste_RuanAlexandre/build.yml?style=flat-square&label=Build)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

**Sistema completo de emissão e gestão de Notas Fiscais com arquitetura de microserviços.**

[🚀 Demo ao Vivo](#-demo) · [📖 Documentação da API](#-documentação-da-api) · [🐛 Reportar Bug](https://github.com/ruanalexandreS/Korp_Teste_RuanAlexandre/issues)

</div>

---

## 📋 Sobre o Projeto

Este projeto foi desenvolvido como **desafio técnico** para a vaga de **Desenvolvedor Jr. C# + Angular** na Korp. O objetivo foi implementar um sistema de emissão de Notas Fiscais com foco em:

- ✅ Arquitetura de microserviços real e independente
- ✅ Resiliência a falhas entre serviços
- ✅ Autenticação JWT e segurança por camadas
- ✅ Boas práticas de engenharia de software
- ✅ Testes unitários com xUnit e Moq
- ✅ Inteligência Artificial integrada via Anthropic Claude

---

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                      KorpApp (Angular)                  │
│              http://localhost:4200                       │
└──────────────────────┬──────────────────────────────────┘
                       │ HTTP + JWT
          ┌────────────┴────────────┐
          ▼                         ▼
┌──────────────────┐     ┌──────────────────────┐
│  ServicoEstoque  │◄────│  ServicoFaturamento  │
│   (porta 7296)   │     │    (porta 7103)       │
│                  │     │                       │
│  CRUD Produtos   │     │  CRUD Notas Fiscais   │
│  Gestão Saldo    │     │  Impressão NF         │
│  EF Core + PG    │     │  JWT Auth             │
│  API Key Guard   │     │  Idempotência         │
└──────────────────┘     └──────────────────────┘
          │                         │
          └───────────┬─────────────┘
                      ▼
            ┌──────────────────┐
            │   PostgreSQL     │
            │  (Railway / PG)  │
            └──────────────────┘
```

### Estrutura da Solução
```
Korp_Teste_RuanAlexandre/
├── ServicoEstoque/           # API REST - Controle de produtos e saldos
├── ServicoFaturamento/       # API REST - Gestão de notas fiscais
├── ServicoFaturamento.Tests/ # Testes unitários (xUnit + Moq)
└── KorpApp/                  # Frontend Angular
```

---

## ✨ Funcionalidades

### Requisitos Obrigatórios
| Feature | Status |
|---|---|
| Arquitetura de Microserviços (2 serviços independentes) | ✅ |
| Cadastro de Produtos (Código, Descrição, Saldo) | ✅ |
| Cadastro de Notas Fiscais com itens | ✅ |
| Impressão de NF com baixa automática no estoque | ✅ |
| Bloqueio de reimpressão de NF fechada | ✅ |
| Resiliência: Estoque offline não derruba Faturamento | ✅ |

### Funcionalidades Adicionais (Bônus)
| Feature | Status |
|---|---|
| Autenticação JWT com perfis (Admin / Operador) | ✅ |
| API Key interna para comunicação entre serviços | ✅ |
| Idempotência via `Idempotency-Key` header | ✅ |
| Controle de concorrência com `SemaphoreSlim` + `ExecuteUpdateAsync` | ✅ |
| Testes unitários com xUnit + Moq (5 testes) | ✅ |
| Integração com IA via Anthropic Claude API | ✅ |
| Paginação no backend (`Skip`/`Take`) | ✅ |
| Middleware global de tratamento de erros | ✅ |
| Reatividade em tempo real via RxJS `Subject` | ✅ |
| Banco configurável por flag (`UseInMemoryDatabase`) | ✅ |
| Retry Policy com 3 tentativas | ✅ |
| Deploy em produção (Railway + Vercel) | ✅ |

---

## 🛠️ Tecnologias

### Backend
- **[.NET 8](https://dotnet.microsoft.com/)** — Framework principal
- **[Entity Framework Core 9](https://learn.microsoft.com/ef/core/)** — ORM
- **[PostgreSQL](https://www.postgresql.org/)** + **[Npgsql](https://www.npgsql.org/)** — Banco de dados em produção
- **[JWT Bearer](https://jwt.io/)** — Autenticação stateless
- **[Swashbuckle / Swagger](https://swagger.io/)** — Documentação da API
- **[xUnit](https://xunit.net/)** + **[Moq](https://github.com/moq/moq4)** — Testes unitários

### Frontend
- **[Angular 17+](https://angular.io/)** — Framework SPA com Standalone Components
- **[Angular Material](https://material.angular.io/)** — Design System
- **[RxJS](https://rxjs.dev/)** — Programação reativa
- **[Reactive Forms](https://angular.io/guide/reactive-forms)** — Formulários com validação

### Infraestrutura
- **[Docker](https://www.docker.com/)** — Containerização
- **[Railway](https://railway.app/)** — Deploy dos backends
- **[Vercel](https://vercel.com/)** — Deploy do frontend
- **[GitHub Actions](https://github.com/features/actions)** — CI/CD

---

## 🧠 Decisões Técnicas

### Por que `IHttpClientFactory` e não `HttpClient` diretamente?
O `HttpClient` instanciado manualmente sofre de *socket exhaustion* em aplicações de longa duração. O `IHttpClientFactory` gerencia o ciclo de vida dos `HttpMessageHandler`, reutilizando conexões TCP de forma eficiente e segura.

### Por que `SemaphoreSlim` para controle de concorrência?
É thread-safe, async-friendly e não bloqueia a thread pool. Garante que apenas uma thread por vez execute a baixa de estoque, evitando race conditions sem precisar de transações distribuídas complexas. Combinado com `ExecuteUpdateAsync`, a atualização é feita diretamente no banco em uma única operação atômica.

### Por que Idempotency Key no header?
Padrão de mercado adotado por Stripe e AWS. Permite que o cliente reenvie a requisição com segurança em caso de timeout ou falha de rede, sem causar efeitos colaterais como cobranças ou baixas de estoque duplicadas.

### Por que JWT Bearer para autenticação?
Stateless por natureza — o servidor não precisa armazenar sessão. Escalável horizontalmente, compatível com microsserviços e padrão de mercado em APIs REST modernas.

### Por que API Key interna no ServicoEstoque?
O ServicoEstoque não deve ser acessível publicamente — apenas pelo ServicoFaturamento. Uma API Key simples no header (`X-Internal-Key`) resolve isso com custo zero de infraestrutura, sem precisar de service mesh ou mTLS para este escopo.

### Decisões Arquiteturais Resumidas

| Decisão | Escolha | Motivo |
|---|---|---|
| Padrão de API | REST | Simplicidade e compatibilidade com Angular |
| Comunicação entre serviços | HTTP + API Key | Leve, sem overhead de message broker |
| Autenticação | JWT Bearer | Stateless, escalável, padrão de mercado |
| Banco de dados | PostgreSQL (prod) / InMemory (dev) | Flexibilidade por ambiente via flag |
| Resiliência | Retry Policy + tratamento HTTP 502 | Falha no Estoque não derruba o Faturamento |
| Idempotência | Header `Idempotency-Key` | Evita duplicação de impressão em retry |
| Concorrência | SemaphoreSlim + ExecuteUpdateAsync | Atualização atômica sem transações distribuídas |

### LINQ utilizado
- `OrderByDescending` + `FirstOrDefaultAsync` — numeração sequencial de notas
- `Skip` / `Take` — paginação na listagem de notas
- `Sum` — total de itens para o resumo da IA
- `Select` — projeção de dados para o prompt da IA
- `Where` + `ExecuteUpdateAsync` — atualização atômica do saldo

---

## 🚀 Demo

| Serviço | URL |
|---|---|
| 🖥️ Frontend (Angular) | `Em produção` |
| ⚙️ API Faturamento (Swagger) | `https://korptesteruanalexandre-production.up.railway.app/swagger` |
| 📦 API Estoque (Swagger) | `https://resilient-delight-production.up.railway.app/swagger` |

---

## ▶️ Como Executar Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js LTS](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) — `npm install -g @angular/cli`
- [Docker](https://www.docker.com/) _(opcional)_

### Opção 1 — Docker Compose _(recomendado)_

```bash
git clone https://github.com/ruanalexandreS/Korp_Teste_RuanAlexandre.git
cd Korp_Teste_RuanAlexandre
docker-compose up
```

Acesse `http://localhost:4200` 🎉

---

### Opção 2 — Manual

**1. Clone o repositório**
```bash
git clone https://github.com/ruanalexandreS/Korp_Teste_RuanAlexandre.git
cd Korp_Teste_RuanAlexandre
```

**2. Configure os segredos locais**

Crie `ServicoFaturamento/appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": true,
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-minimo-32-caracteres",
    "Issuer": "ServicoFaturamento",
    "Audience": "KorpApp",
    "ExpirationHours": 8
  },
  "InternalApiKey": "sua-chave-interna-secreta"
}
```

Crie `ServicoEstoque/appsettings.Development.json`:
```json
{
  "UseInMemoryDatabase": true,
  "InternalApiKey": "sua-chave-interna-secreta"
}
```

**3. Suba os backends**
```bash
# Terminal 1
cd ServicoEstoque && dotnet run

# Terminal 2
cd ServicoFaturamento && dotnet run
```

**4. Suba o frontend**
```bash
cd KorpApp
npm install
ng serve -o
```

Acesse `http://localhost:4200` 🎉

---

## 🧪 Testes

```bash
cd ServicoFaturamento.Tests
dotnet test
```

**Resultado esperado:** 5 testes, 0 falhas.

| Teste | Cenário |
|---|---|
| `CriarNota_DeveRetornarCreated` | Criação válida de NF |
| `NumeracaoSequencial_DeveIncrementar` | Sequência de numeração |
| `ImprimirNota_StatusFechada_DeveRetornarBadRequest` | Bloqueio de reimpressão |
| `ImprimirNota_ComIdempotencyKey_NaoDeveReprocessar` | Idempotência |
| `ImprimirNota_EstoqueOffline_DeveRetornar502` | Resiliência a falhas |

---

## 🧪 Fluxo de Teste Manual

### ✅ Fluxo de Sucesso
```
1. POST /auth/login → { "usuario": "admin", "senha": "admin123" }
2. Copie o token JWT retornado
3. Use o token: Authorization: Bearer {token}
4. POST /api/produtos → cadastre um produto
5. POST /notas → crie uma Nota Fiscal
6. POST /notas/{id}/imprimir → imprima a NF
   ✓ Status muda para "Fechada"
   ✓ Saldo do produto é atualizado
   ✓ IA gera resumo profissional da nota
```

### ❌ Fluxo de Falha (Resiliência)
```
1. Cadastre um produto e uma NF
2. Pare o ServicoEstoque
3. Tente imprimir a NF
   ✓ Sistema retorna erro 502
   ✓ NF permanece "Aberta"
   ✓ Nenhum dado é corrompido
```

### 🤖 Sugestão de Estoque via IA
```http
GET /api/produtos/sugestao-estoque
Authorization: Bearer {token}
```
Retorna análise inteligente com recomendações de reposição baseadas no estoque atual.

---

## 📖 Documentação da API

### Autenticação

```http
POST /auth/login
Content-Type: application/json

{
  "usuario": "admin",
  "senha": "admin123"
}
```

| Usuário | Senha | Perfil |
|---|---|---|
| admin | admin123 | Administrador |
| operador | op123 | Operador |

### Endpoints

| Método | Endpoint | Descrição | Auth |
|---|---|---|---|
| `POST` | `/auth/login` | Gera token JWT | ❌ |
| `GET` | `/api/produtos` | Lista produtos | ✅ |
| `POST` | `/api/produtos` | Cria produto | ✅ |
| `GET` | `/api/produtos/sugestao-estoque` | Sugestão via IA | ✅ |
| `GET` | `/notas/listar` | Lista NFs paginado | ✅ |
| `POST` | `/notas` | Cria Nota Fiscal | ✅ |
| `GET` | `/notas/{id}` | Busca NF por ID | ✅ |
| `POST` | `/notas/{id}/imprimir` | Imprime NF | ✅ |

---

## 🔐 Segurança

- **JWT Authentication** — Todos os endpoints protegidos (exceto `/auth/login`)
- **API Key interna** — ServicoEstoque só aceita chamadas via `X-Internal-Key`
- **Variáveis de ambiente** — Nenhuma credencial no repositório
- **CORS configurado** — Apenas origens autorizadas
- **Idempotência** — Proteção contra double-submit via `Idempotency-Key`
- **Concorrência** — `SemaphoreSlim` previne race conditions na baixa de estoque

---

## 👤 Autor

**Ruan Alexandre dos Santos Campos**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ruan-alexandre-/)
[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/ruanalexandreS)
[![Portfolio](https://img.shields.io/badge/Portfolio-FF5722?style=for-the-badge&logo=google-chrome&logoColor=white)](https://portfolio-ruan-alexandre-s.vercel.app)


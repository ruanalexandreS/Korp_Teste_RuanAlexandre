# Teste Prático Korp - Desenvolvedor Jr. (C\# + Angular)

Este repositório contém a solução completa para o desafio técnico da Korp, implementando um sistema de emissão de notas fiscais com uma arquitetura de microsserviços em .NET e um frontend em Angular.

## 🚀 Funcionalidades Implementadas

O projeto atende a todos os requisitos obrigatórios e inclui funcionalidades adicionais que demonstram boas práticas de engenharia de software.

### Requisitos Obrigatórios

  * ✅ **Arquitetura de Microsserviços:** O backend é dividido em dois serviços independentes: `ServicoEstoque` e `ServicoFaturamento`.
  * ✅ **Cadastro de Produtos:** O frontend permite o cadastro de novos produtos (Código, Descrição, Saldo) que são persistidos no `ServicoEstoque`.
  * ✅ **Cadastro de Notas Fiscais:** O frontend permite a criação de novas notas fiscais (com um ou mais itens) que são persistidas no `ServicoFaturamento`.
  * ✅ **Impressão de Notas Fiscais:**
      * O botão "Imprimir" chama o `ServicoFaturamento`.
      * O `ServicoFaturamento` chama o `ServicoEstoque` para dar baixa no saldo dos produtos.
      * O status da nota é atualizado para "Fechada".
      * Não é permitido imprimir notas que não estejam "Abertas".
  * ✅ **Tratamento de Falhas:** A aplicação é resiliente. Se o `ServicoEstoque` estiver offline durante uma impressão, o `ServicoFaturamento` reporta o erro, a nota permanece "Aberta" e o frontend exibe uma notificação de falha ao usuário.

### ✨ Funcionalidades Adicionais (Bônus)

  * **Reatividade em Tempo Real:** A lista de produtos e a lista de notas se atualizam automaticamente na tela após um novo cadastro (sem a necessidade de F5), usando `Subject` do RxJS.
  * **Middleware de Erros:** O backend `ServicoFaturamento` possui um `ErrorHandlingMiddleware` global que captura todas as exceções não tratadas e as formata em uma resposta JSON padronizada.
  * **Validação Avançada:** Os `Models` do backend (C\#) e os `Forms` do frontend (Angular) usam validação de dados (`[Required]`, `[Range]`, `Validators.required`).
  * **Banco de Dados Configurável:** O `ServicoFaturamento` está configurado para alternar entre `InMemoryDatabase` (para testes) e `SQL Server` (para produção) com base em uma única flag no `appsettings.json`.
  * **Paginação no Backend:** A API de listagem de notas (`GET /listar`) possui lógica de paginação (`Skip`/`Take`).

-----

## 🛠️ Arquitetura e Tecnologias

A solução é dividida em 3 projetos principais que rodam de forma independente:

1.  **`ServicoEstoque` (Backend - C\#):**

      * API REST em .NET 8.
      * Responsável por todo o CRUD de Produtos e atualização de saldo.
      * Utiliza **EF Core** com **InMemory Database**.
      * Documentado com **Swagger**.
      * Configurado com **CORS** para permitir chamadas do frontend.

2.  **`ServicoFaturamento` (Backend - C\#):**

      * API REST em .NET 8.
      * Responsável pelo CRUD de Notas Fiscais.
      * Utiliza `IHttpClientFactory` para se comunicar com o `ServicoEstoque`.
      * Usa **EF Core** com lógica para alternar entre **InMemory** e **SQL Server**.
      * Possui o **Middleware de Tratamento de Erros**.

3.  **`KorpApp` (Frontend - Angular):**

      * Aplicação em **Angular** (standalone components).
      * Utiliza **Angular Material** para a interface (Tabelas, Formulários, Botões, Cards, Notificações).
      * Usa `HttpClientModule` para consumir as duas APIs.
      * Usa **RxJS** (`Subject`, `subscribe`) para reatividade da UI.
      * Usa **Reactive Forms** para validação dos formulários.

-----

## ▶️ Como Executar o Projeto

Para rodar este projeto, você precisará ter os dois backends (APIs) e o frontend (Angular) rodando simultaneamente.

### Pré-requisitos

  * Visual Studio 2022 (com a carga de trabalho .NET 8)
  * Node.js (LTS)
  * Angular CLI (`npm install -g @angular/cli`)
  * Visual Studio Code (Recomendado para o Angular)

### 1\. 🚀 Rodando o Backend (Serviço de Estoque e Faturamento)

1.  Abra o arquivo `korp_Teste_RuanAlexandre.sln` no **Visual Studio 2022**.
2.  No "Gerenciador de Soluções", clique com o botão direito na **Solução** (`Solução 'korp_Teste_RuanAlexandre'`).
3.  Vá em **"Definir Projetos de Inicialização..."**.
4.  Selecione **"Vários projetos de inicialização"**.
5.  Defina a "Ação" como **"Iniciar"** para `ServicoEstoque` e `ServicoFaturamento`.
6.  Clique em "Aplicar" e "OK".
7.  Aperte o botão de "Play" (▶) (o triângulo verde) na barra de ferramentas.

**Resultado:** Duas janelas de terminal (ou Swagger) devem abrir, confirmando que as duas APIs estão rodando. Anote as portas (ex: `https://localhost:7296` para Estoque e `https://localhost:7103` para Faturamento).

### 2\. 🖥️ Rodando o Frontend (Aplicação Angular)

1.  Abra a pasta raiz do projeto (`korp_Teste_RuanAlexandre`) no **Visual Studio Code**.
2.  Abra um novo terminal (no menu `Terminal` \> `Novo Terminal`).
3.  Navegue até a pasta do frontend:
    ```bash
    cd KorpApp
    ```
4.  Instale as dependências (só na primeira vez):
    ```bash
    npm install
    ```
5.  Rode o servidor do Angular:
    ```bash
    ng serve -o
    ```

**Resultado:** O seu navegador abrirá automaticamente no `http://localhost:4200` com a aplicação completa.

> **Nota:** Se a aplicação não conseguir se conectar ao backend, verifique as portas (URLs) nos arquivos de serviço do Angular:
>
>   * `KorpApp/src/app/services/estoque.service.ts` (deve apontar para a porta do `ServicoEstoque`)
>   * `KorpApp/src/app/services/faturamento.service.ts` (deve apontar para a porta do `ServicoFaturamento`)

-----

## 🧪 Como Testar os Requisitos (Demonstração)

Com os 3 projetos rodando:

### Fluxo de Sucesso (Impressão)

1.  Na tela `localhost:4200`, cadastre um novo produto (ex: "Produto A", Saldo "20").
2.  O produto aparecerá na tabela "Produtos Cadastrados" (o ID dele será `1`).
3.  No formulário de "Nota Fiscal", adicione um item (Produto ID: `1`, Quantidade: `5`).
4.  Clique em "Salvar Nota Fiscal".
5.  A nota aparecerá na tabela "Notas Fiscais Cadastradas" com o status **"Aberta"**.
6.  Clique no botão "Imprimir" (🖨️) dessa nota.
7.  **Resultado:** Você verá a notificação de sucesso, o status da nota mudará para **"Fechada"** e o botão "Imprimir" ficará desabilitado. (Se recarregar a página, o saldo do "Produto A" estará `15`).

### Fluxo de Falha (Requisito Obrigatório)

1.  Cadastre um novo produto (ex: "Produto B", Saldo "10").
2.  Cadastre uma nova Nota Fiscal para ele (Produto ID: `2`, Quantidade: `1`). A nota aparecerá como **"Aberta"**.
3.  Vá até o **Visual Studio 2022** e **pare** (botão "Stop" ⏹) o projeto `ServicoEstoque`.
4.  Volte ao `localhost:4200` e clique no botão "Imprimir" (🖨️) da Nota 2.
5.  **Resultado:** Você verá uma **notificação de erro** (ex: "Falha ao atualizar o estoque"). A nota **permanecerá "Aberta"** e o botão "Imprimir" continuará habilitado, provando que o sistema tratou a falha do microsserviço.
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ServicoFaturamento.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = GetStatusCode(ex),
                Mensagem = GetMensagem(ex),
                Detalhes = ex.Message
            };

            context.Response.StatusCode = response.Status;
            await context.Response.WriteAsJsonAsync(response);
        }

        private static int GetStatusCode(Exception ex) => ex switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,            //404
            InvalidOperationException => (int)HttpStatusCode.BadRequest,    //400
            HttpRequestException => (int)HttpStatusCode.BadGateway,        //502
            DbUpdateException => (int)HttpStatusCode.InternalServerError, //500
            _ => (int)HttpStatusCode.InternalServerError                 //500
        };

        private static string GetMensagem(Exception ex) => ex switch
        {
            KeyNotFoundException => "Recurso não encontrado.",
            InvalidOperationException => "Operação inválida.",
            HttpRequestException => "Erro ao comunicar com serviço externo.",
            DbUpdateException => "Erro ao atualizar o banco de dados.",
            _ => "Erro interno do servidor."
        };
    }
}

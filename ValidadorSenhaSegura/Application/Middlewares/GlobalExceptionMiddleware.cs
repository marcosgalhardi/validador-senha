using Microsoft.AspNetCore.Mvc;

namespace ValidadorSenhaSegura.Application.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception capturada pelo middleware global.");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Erro interno no servidor",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path,
            };

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

}

using ValidadorSenhaSegura.Application.Middlewares;

namespace ValidadorSenhaSegura.Application.Configuration
{
    public static class RegisterMiddlewares
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }

}

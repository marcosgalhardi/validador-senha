using Asp.Versioning;
using Asp.Versioning.Builder;

namespace ValidadorSenhaSegura.Application.Configuration
{
    public static class RegisterApiVersions
    {
        public static ApiVersionSet GetInstanceApiVersionSet(this IEndpointRouteBuilder app)
        {
            // Criando as versões da API
            return app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(2, 0))
                .ReportApiVersions()
                .Build();
        }
    }
}

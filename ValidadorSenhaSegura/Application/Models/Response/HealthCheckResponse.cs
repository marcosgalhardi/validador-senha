using ValidadorSenhaSegura.Application.Dto;

namespace ValidadorSenhaSegura.Application.Models.Response
{
    public record HealthCheckResponse : GenericModel
    {
        public required bool Liveness { get; init; }

        public required bool Readiness { get; init; }
    }
}

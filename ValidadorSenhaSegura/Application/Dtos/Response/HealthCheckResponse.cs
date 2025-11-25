namespace ValidadorSenhaSegura.Application.Dtos.Response
{
    public record HealthCheckResponse : GenericModel
    {
        public required bool Liveness { get; init; }

        public required bool Readiness { get; init; }
    }
}

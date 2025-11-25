namespace ValidadorSenhaSegura.Application.Dtos.Request
{
    public record ValidatePasswordRequest
    {
        public required string Password { get; init; }
    }
}

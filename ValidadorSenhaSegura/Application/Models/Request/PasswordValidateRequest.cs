namespace ValidadorSenhaSegura.Application.Models.Request
{
    public record PasswordValidateRequest
    {
        public required string Password { get; init; }
    }
}

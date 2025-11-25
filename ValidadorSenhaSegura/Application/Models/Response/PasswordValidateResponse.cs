using ValidadorSenhaSegura.Application.Dto;

namespace ValidadorSenhaSegura.Application.Models.Response
{

    public record PasswordValidateResponse : GenericModel<string>
    {
        public required string ApiVersion { get; init; }
    }
}

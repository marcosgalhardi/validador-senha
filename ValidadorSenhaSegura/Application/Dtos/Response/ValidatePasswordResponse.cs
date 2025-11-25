namespace ValidadorSenhaSegura.Application.Dtos.Response
{

    public record ValidatePasswordResponse : GenericModel<string>
    {
        public required string ApiVersion { get; init; }
    }
}

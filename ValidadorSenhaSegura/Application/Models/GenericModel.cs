using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Application.Dto
{
    public record GenericModel<TOut> : GenericModel
    {
        public required TOut Data { get; init; }
    }

    public record GenericModel
    {
        public required List<Error> Errors { get; init; }

        public bool IsValid => Errors.Count < 1;
    }

}

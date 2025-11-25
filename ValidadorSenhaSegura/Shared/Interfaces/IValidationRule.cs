using ValidadorSenhaSegura.Domain.Enums;

namespace ValidadorSenhaSegura.Shared.Interfaces
{
    public interface IValidationRule<T>
    {
        string ErrorMessage { get; }
        public RulesValidationErrorCode ErrorCode { get; }
        public bool ContinueIfErrorOccurs { get; }
        bool IsValid(T input);
    }
}

using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class NullNotAllowed : IValidationRule<string>
    {
        public string ErrorMessage => $"Nulo não é permitido.";
        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.NullNotAllowed;
        public bool ContinueIfErrorOccurs => false;
        public bool IsValid(string input) => input != null;
    }
}

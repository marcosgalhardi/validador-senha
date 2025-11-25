using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class MustContainUppercaseRule : IValidationRule<string>
    {
        public string ErrorMessage => "Deve ter letra maiúscula.";

        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.MustContainUppercaseRuleValidation;

        public bool ContinueIfErrorOccurs => false;
        public bool IsValid(string input) => input.Any(char.IsUpper);
    }
}

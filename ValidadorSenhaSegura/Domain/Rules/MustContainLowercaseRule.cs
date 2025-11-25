using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class MustContainLowercaseRule : IValidationRule<string>
    {
        public string ErrorMessage => "Deve ter letra minúscula.";

        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.MustContainLowercaseRuleValidation;

        public bool ContinueIfErrorOccurs => false;

        public bool IsValid(string input) => input.Any(char.IsLower);
    }
}

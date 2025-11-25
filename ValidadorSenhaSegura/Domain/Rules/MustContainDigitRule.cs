using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class MustContainDigitRule : IValidationRule<string>
    {
        public string ErrorMessage => "Deve ter ao menos um número.";

        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.MustContainDigitRuleValidation;

        public bool ContinueIfErrorOccurs => true;

        public bool IsValid(string input) => input.Any(char.IsDigit);
    }
}

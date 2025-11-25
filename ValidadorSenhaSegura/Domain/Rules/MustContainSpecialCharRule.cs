using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class MustContainSpecialCharRule : IValidationRule<string>
    {
        private static readonly HashSet<char> SpecialChars = 
            ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+'];

        public string ErrorMessage => "Deve ter caractere especial.";

        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.MustContainSpecialCharRuleValidation;

        public bool ContinueIfErrorOccurs => true;

        public bool IsValid(string input) =>
            input.Any(c => SpecialChars.Contains(c));
    }
}

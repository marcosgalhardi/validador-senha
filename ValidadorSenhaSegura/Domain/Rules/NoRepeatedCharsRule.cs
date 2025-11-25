using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class NoRepeatedCharsRule : IValidationRule<string>
    {
        public string ErrorMessage => "Não pode ter caracteres repetidos.";
        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.NoRepeatedCharsRuleValidation;

        public bool ContinueIfErrorOccurs => false;

        public bool IsValid(string input) =>
            input.Length == input.Distinct().Count();
    }
}

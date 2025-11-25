using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class WhitespaceNotAllowed : IValidationRule<string>
    {
        public string ErrorMessage => $"Espaço em branco é um caractere inválido.";
        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.WhitespaceNotAllowed;

        public bool ContinueIfErrorOccurs => true;

        public bool IsValid(string input) => !input.Contains(' ');
    }
}

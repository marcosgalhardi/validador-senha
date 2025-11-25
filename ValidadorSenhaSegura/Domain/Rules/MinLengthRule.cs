using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Domain.Rules
{
    public class MinLengthRule : IValidationRule<string>
    {
        private readonly int _min;

        public MinLengthRule(int min)
        {
            _min = min;
        }

        public string ErrorMessage => $"Deve ter no mínimo {_min} caracteres.";
        public RulesValidationErrorCode ErrorCode => RulesValidationErrorCode.MinLengthRuleValidation;

        public bool ContinueIfErrorOccurs => true;


        public bool IsValid(string input) => input?.Length >= _min;
    }
}

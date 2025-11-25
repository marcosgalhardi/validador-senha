using ValidadorSenhaSegura.Shared.Interfaces;

namespace ValidadorSenhaSegura.Shared.Abstractions
{
    public class Validator<T>
    {
        private readonly List<IValidationRule<T>> _rules = [];

        public Validator<T> AddRule(IValidationRule<T> rule)
        {
            _rules.Add(rule);

            return this;
        }

        public ValidationResult Validate(T input)
        {
            List<Error> errors = [];

            foreach (var rule in _rules)
            {
                var isValid = rule.IsValid(input);

                if (!isValid)
                {
                    errors.Add(Error.Create(rule.ErrorMessage, rule.ErrorCode));

                    if (!rule.ContinueIfErrorOccurs)
                        break;
                }
            }

            return ValidationResult.Failure(errors);
        }
    }
}

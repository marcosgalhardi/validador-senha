using System.Text.RegularExpressions;
using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Domain.Validators
{
    public class RulesetPasswordValidatorV1 : IRulesetPasswordValidator
    {
        // Regex para verificar os requisitos básicos
        private readonly Regex _basicRegex = new(
            @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()\-\+])[A-Za-z0-9!@#$%^&*()\-\+]{9,}$",
            RegexOptions.Compiled);

        public ApiVersion ApiVersion => ApiVersion.V1_;

        public ValidationResult Validate(string password)
        {
            int minLength = 9;
            List<Error> errors = new();

            if (string.IsNullOrWhiteSpace(password) || !HasMinimumLength(password, minLength))
            {
                errors.Add(Error.Create($"Deve ter no mínimo {minLength} caracteres.", RulesetValidationErrorCode.MinLengthRuleValidation));
            }

            // Não permite caracteres repetidos
            if (string.IsNullOrWhiteSpace(password) || HasRepeatedCharacters(password))
                errors.Add(Error.Create("A senha não pode conter caracteres repetidos.", RulesetValidationErrorCode.NoRepeatedCharsRuleValidation));

            // Não permite espaços
            if (string.IsNullOrWhiteSpace(password) || password.Any(char.IsWhiteSpace))
                errors.Add(Error.Create("A senha não deve conter espaços.", RulesetValidationErrorCode.WhitespaceNotAllowed));
            
            // Valida regras básicas com Regex
            if (string.IsNullOrWhiteSpace(password) || !_basicRegex.IsMatch(password))
                errors.Add(Error.Create("A senha não atende aos requisitos mínimos.", RulesetValidationErrorCode.MinimumRequirementsRuleValidation));

            return GetValidationResult(errors);
        }
        private bool HasRepeatedCharacters(string input)
        {
            return input
                .GroupBy(c => c)
                .Any(g => g.Count() > 1);
        }

        private bool HasMinimumLength(string input, int minLength)
        {
            return input.Length >= minLength;
        }

        private ValidationResult GetValidationResult(List<Error> errors)
        {
            return errors.Count == 0 ?
                ValidationResult.Success() :
                ValidationResult.Failure(errors);
        }
    }
}

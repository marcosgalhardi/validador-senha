using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;
using ValidadorSenhaSegura.Shared.Abstractions;

namespace ValidadorSenhaSegura.Domain.Validators.Password
{
    public class RulesetPasswordValidatorV2 : IRulesetPasswordValidator
    {
        public ApiVersion ApiVersion => ApiVersion.V2_;

        private readonly Validator<string> _validator;

        public RulesetPasswordValidatorV2()
        {
            _validator = new Validator<string>()
                .AddRule(new NullNotAllowed())
                .AddRule(new WhitespaceNotAllowed())
                .AddRule(new MinLengthRule(9))
                .AddRule(new MustContainDigitRule())
                .AddRule(new MustContainLowercaseRule())
                .AddRule(new MustContainUppercaseRule())
                .AddRule(new MustContainSpecialCharRule())
                .AddRule(new NoRepeatedCharsRule());
        }

        public ValidationResult Validate(string password)
        {
            var result = _validator.Validate(password);

            return  result.IsValid ? 
                ValidationResult.Success() : 
                ValidationResult.Failure(result.Errors);
        }
    }
}

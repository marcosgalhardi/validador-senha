using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Application.Validators.Password
{
    public class PasswordValidatorV2 : IPasswordValidator
    {
        private readonly IRulesetPasswordValidator _rulesetPasswordValidator;

        public PasswordValidatorV2(IEnumerable<IRulesetPasswordValidator> rulesetPasswordValidators)
        {
            _rulesetPasswordValidator = rulesetPasswordValidators.Single(t => t.ApiVersion == ApiVersion);
        }

        public ApiVersion ApiVersion => ApiVersion.V2_;

        public ValidationResult Validate(string password)
        {
            var result = _rulesetPasswordValidator.Validate(password);

            return result.IsValid ?
               ValidationResult.Success() :
               ValidationResult.Failure(result.Errors);
        }
    }
}

using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Application.Validators.Password
{
    public class PasswordValidatorV1 : IPasswordValidator
    {
        private readonly IRulesetPasswordValidator _rulesetPasswordValidator;

        public PasswordValidatorV1(IEnumerable<IRulesetPasswordValidator> rulesetPasswordValidators)
        {
            _rulesetPasswordValidator = rulesetPasswordValidators.Single(t => t.ApiVersion == ApiVersion);
        }

        public ApiVersion ApiVersion => ApiVersion.V1_;

        public ValidationResult Validate(string password)
        {
            var result = _rulesetPasswordValidator.Validate(password);

            return result.IsValid ?
               ValidationResult.Success() :
               ValidationResult.Failure(result.Errors);
        }
    }
}

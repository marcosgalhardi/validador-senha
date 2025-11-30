using ValidadorSenhaSegura.Application.Dtos.Response;
using ValidadorSenhaSegura.Application.UseCases.Interfaces;
using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Domain.ValueObjects;

namespace ValidadorSenhaSegura.Application.UseCases
{
    public class UseCasePasswordValidate : IUseCasePasswordValidate
    {
        private readonly IEnumerable<IPasswordValidator> _passwordValidators;
        
        private IPasswordValidator _passwordValidator = null!;
        private ApiVersion _apiVersion;

        public UseCasePasswordValidate(IEnumerable<IPasswordValidator> passwordValidators)
        {
            _passwordValidators = passwordValidators;
        }

        public IUseCasePasswordValidate SetStrategy(ApiVersion apiVersion)
        {
            _apiVersion = apiVersion;

            _passwordValidator =
                _passwordValidators.Single(t => t.ApiVersion == _apiVersion);

            return this;
        }

        public ValidatePasswordResponse Execute(string password)
        {
            // ✅ MELHORIA: Remoção de espaços em branco conforme requisito
            var cleanPassword = password.Replace(" ", string.Empty);

            var resultPassword = Password.Create(cleanPassword, _passwordValidator);

            var viewModel = new ValidatePasswordResponse
            {
                ApiVersion = ((int)_apiVersion).ToString(),

                Data = resultPassword.IsSuccess?
                    "A senha informada é válida" :
                    "A senha informada é inválida, pois não atende aos critérios",

                Errors = resultPassword.Errors
            };

            return viewModel;
        }
    }
}
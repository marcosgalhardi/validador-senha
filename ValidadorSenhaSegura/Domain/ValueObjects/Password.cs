using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Domain.ValueObjects
{
    public record Password
    {
        private readonly string _password;

        private Password(string password)
        { 
            _password = password;
        }

        public override string ToString()
        {
            return _password;
        }

        public static Result<Password> Create(
            string password, 
            IPasswordValidator passwordValidator) 
        {
            var result = passwordValidator.Validate(password);

            return result.IsValid ?
                Result.Success(new Password(password)) :
                Result.Failure<Password>(result.Errors);
        }
    }
}

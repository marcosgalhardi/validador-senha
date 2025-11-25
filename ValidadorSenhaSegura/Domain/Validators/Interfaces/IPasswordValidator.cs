using ValidadorSenhaSegura.Domain.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Domain.Validators.Interfaces
{
    public interface IPasswordValidator : IApiVersion
    {
        ValidationResult Validate(string password);
    }
}
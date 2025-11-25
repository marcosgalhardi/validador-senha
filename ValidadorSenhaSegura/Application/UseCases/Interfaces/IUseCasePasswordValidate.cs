using ValidadorSenhaSegura.Application.Models.Response;
using ValidadorSenhaSegura.Domain.Enums;

namespace ValidadorSenhaSegura.Application.UseCases.Interfaces
{
    public interface IUseCasePasswordValidate
    {
        PasswordValidateResponse Execute(string password);
        IUseCasePasswordValidate SetStrategy(ApiVersion apiVersion);
    }
}
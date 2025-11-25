using ValidadorSenhaSegura.Application.Dtos.Response;
using ValidadorSenhaSegura.Domain.Enums;

namespace ValidadorSenhaSegura.Application.UseCases.Interfaces
{
    public interface IUseCasePasswordValidate
    {
        ValidatePasswordResponse Execute(string password);
        IUseCasePasswordValidate SetStrategy(ApiVersion apiVersion);
    }
}
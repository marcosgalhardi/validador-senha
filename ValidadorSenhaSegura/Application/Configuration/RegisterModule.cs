using Asp.Versioning.Builder;
using ValidadorSenhaSegura.Application.UseCases;
using ValidadorSenhaSegura.Application.UseCases.Interfaces;
using ValidadorSenhaSegura.Application.Validators.Password;
using ValidadorSenhaSegura.Domain.Validators;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Domain.Validators.Password;

namespace ValidadorSenhaSegura.Application.Configuration
{
    public static class RegisterModule
    {
        public static IApplicationBuilder AddRoutes(this IEndpointRouteBuilder app)
        {
            ApiVersionSet versionSet = app.GetInstanceApiVersionSet();

            app.AddRoutes(versionSet);

            return (IApplicationBuilder)app;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordValidator, PasswordValidatorV1>();
            services.AddSingleton<IPasswordValidator, PasswordValidatorV2>();
            services.AddSingleton<IUseCasePasswordValidate, UseCasePasswordValidate>();

            return services;
        }

        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddSingleton<IRulesetPasswordValidator, RulesetPasswordValidatorV1>();
            services.AddSingleton<IRulesetPasswordValidator, RulesetPasswordValidatorV2>();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {           
            return services;
        }
    }
}

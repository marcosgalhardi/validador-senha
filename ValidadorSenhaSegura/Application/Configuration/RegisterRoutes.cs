using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using ValidadorSenhaSegura.Application.Dtos.Request;
using ValidadorSenhaSegura.Application.Dtos.Response;
using ValidadorSenhaSegura.Application.UseCases.Interfaces;
using ValidadorSenhaSegura.Shared;

namespace ValidadorSenhaSegura.Application.Configuration
{
    public static class RegisterRoutes
    {
        public static IApplicationBuilder AddRoutes(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            AddRouteValidatePassword(app, versionSet);
            AddRouteHealthCheck(app, versionSet);
            AddRouteThrowException(app, versionSet);

            return (IApplicationBuilder)app;
        }

        private static void AddRouteThrowException(IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapGet("/api/throw-exception", () =>
            {
                throw new Exception("Exception lançada na rota...");
            })
                        .WithApiVersionSet(versionSet)
                        .MapToApiVersion(new Asp.Versioning.ApiVersion(1, 0))
                        .WithName("ThrowExceptionV1")
                        .WithOpenApi()
                        ;

            app.MapGet("/api/throw-exception", () =>
            {
                throw new Exception("Exception lançada na rota...");
            })
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new Asp.Versioning.ApiVersion(2, 0))
            .WithName("ThrowExceptionV2")
            .WithOpenApi()
            .Produces<ValidatePasswordResponse>(StatusCodes.Status200OK)
            .Produces<ValidatePasswordResponse>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails?>(StatusCodes.Status500InternalServerError);
            
        }

        private static void AddRouteHealthCheck(IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapGet("/api/hc", () =>
            {
                // Demonstra aplicação viva e simula estar pronta para ser consumida.
                return Results.Ok(new HealthCheckResponse { 
                    Liveness = true, Readiness = true, Errors = new List<Error>() 
                });
            })
                        .WithApiVersionSet(versionSet)
                        .MapToApiVersion(new Asp.Versioning.ApiVersion(1, 0))
                        .WithName("HcV1")
                        .WithOpenApi();

            app.MapGet("/api/hc", () =>
            {
                // Demonstra aplicação viva e simula estar pronta para ser consumida.
                return Results.Ok(new HealthCheckResponse { 
                    Liveness = true, Readiness = true, Errors = new List<Error>() 
                });
            })
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new Asp.Versioning.ApiVersion(2, 0))
            .WithName("HcV2")
            .WithOpenApi()
            .Produces<ValidatePasswordResponse>(StatusCodes.Status200OK)
            .Produces<ValidatePasswordResponse>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails?>(StatusCodes.Status500InternalServerError);
        }

        private static void AddRouteValidatePassword(IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapPost("/api/validate-password", (
                            [FromBody] ValidatePasswordRequest validatePasswordRequest,
                            [FromServices] IUseCasePasswordValidate useCase) =>
            {
                useCase.SetStrategy(Domain.Enums.ApiVersion.V1_);
                var result = useCase.Execute(validatePasswordRequest.Password);

                return result.IsValid ?
                    Results.Ok(result) :
                    Results.BadRequest(result);
            })
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(new Asp.Versioning.ApiVersion(1, 0))
            .WithName("ValidatePasswordV1")
            .WithOpenApi()
            .Produces<ValidatePasswordResponse>(StatusCodes.Status200OK)
            .Produces<ValidatePasswordResponse>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails?>(StatusCodes.Status500InternalServerError);


            app.MapPost("/api/validate-password", (
               [FromBody] ValidatePasswordRequest validatePasswordRequest,
               [FromServices] IUseCasePasswordValidate useCase) =>
            {
                useCase.SetStrategy(Domain.Enums.ApiVersion.V2_);
                var result = useCase.Execute(validatePasswordRequest.Password);

                return result.IsValid ?
                    Results.Ok(result) :
                    Results.BadRequest(result);
            })
           .WithApiVersionSet(versionSet)
           .MapToApiVersion(new Asp.Versioning.ApiVersion(2, 0))
           .WithName("ValidatePasswordV2")
           .WithOpenApi()
           .Produces<ValidatePasswordResponse>(StatusCodes.Status200OK)
           .Produces<ValidatePasswordResponse>(StatusCodes.Status400BadRequest)
           .Produces<ProblemDetails?>(StatusCodes.Status500InternalServerError);
        }
    }
}

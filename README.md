# ValidadorSenhaSegura

API minimalista para validação de senhas construída com .NET 9 (C# 13). 
Projetada com separação de responsabilidades, versionamento por cabeçalho e suíte de testes unitários e testes integrados.

## Destaques / Pontos positivos

- Minimal API (.NET 8) — inicialização rápida mais rápida que uma WebApi devido a ter menos recursos e etc.
  isto visa permitir um menor coldStart.

- Optei pela versão 8.0 por já conhecer e também pelo fato de ser uma versão LTS.

- Optei pelo versionamento da API com Asp.Versioning usando cabeçalho `api-version` (permite múltiplas versões na mesma rota), 
  essa decisão foi tomada visto que para uso com API Gateway, pode fazer muito sentido.

- Swagger/OpenAPI integrado e com separação de versões (geração de docs por versão).

- Arquitetura modular:
  - Camadas separadas: Application, Domain, Infrastructure e Shared.
  - Use-cases explícitos (IUseCasePasswordValidate / UseCasePasswordValidate).
  - Validações implementadas como regras isoladas (Single Responsibility).

- Seleção de validador por estratégia:
  - UseCasePasswordValidate escolhe a implementação IPasswordValidator baseada na versão em uma interface fluent e também o design pattern 'Strategy'.

- Modelo de validação robusto:
  - ValidationResult + Error permite retorno estruturado de erros.
  - RulesetPasswordValidator usa Regex compilada para requisitos básicos (melhor desempenho do que se fosse interpretada).

- Middleware de tratamento global de exceções (centraliza respostas de erro, qualquer exception não tratada será capturada aqui).

- Endpoint de health-check (`/api/hc`) para liveness/readiness, eventual uso no gerenciamento da instância.

- Endpoint de throw-exception (`/api/throw-exception`) para demonstrar que a aplicação não cai, 
  mesmo sem um try/catch devido ao middleware criado para tratamento de erros.

- Boa cobertura de testes unitários com xUnit + Moq (regras e use-cases testados).

- Atualmente o Swagger é ativado apenas em ambiente de desenvolvimento. este poder ser alterado facilmente se assim desejado.

## Como rodar (rápido)

- Build: use __Build Solution__ no Visual Studio ou `dotnet build` na raiz da solução.
- Run: use __Start Debugging__ / __Run__ ou `dotnet run --project ValidadorSenhaSegura`.
- Testes: use __Test Explorer__ ou `dotnet test` na raiz.

## Detalhes de implementação notáveis
- A implementação de versionamento foi feito a efeito de demontração no intuito de apresentar 
  tecnicamente como resolvi as situações que este contexto trouxe (Eu sei, devo evitar overengineer, 
  mas nesse caso optei por ser uma avaliação).

- Versão padrão da API está configurada como 2.0 e `AssumeDefaultVersionWhenUnspecified = true`.

- Swagger é configurado para emitir um documento por versão usando `IApiVersionDescriptionProvider`.

- Regras de validação são classes pequenas e testáveis (MinLengthRule, MustContainDigitRule, etc.).

- RulesetPasswordValidator aplica: mínimo de caracteres, sem repetição de caracteres, sem espaços e requisitos básicos via regex.

## Suíte de testes

Abaixo são listados os testes existentes no repositório divididos entre testes unitários e testes integrados.

## Middlewares

Abaixo uma tabela explicando o middleware global da aplicação e como ele é utilizado.

| Middleware | Arquivo | Registrado em | Objetivo | Comportamento | Resposta HTTP | Logs / Observações |
|---|---:|---|---|---|---:|---|
| GlobalExceptionMiddleware | ValidadorSenhaSegura.Application.Middlewares.GlobalExceptionMiddleware.cs | Chamado em Program.cs via _app.UseGlobalExceptionHandler()_ (extensão em Application.Configuration.RegisterMiddlewares) | Capturar exceções não tratadas no pipeline e retornar uma resposta padronizada ao cliente | Envolve a chamada ao próximo middleware/endpoint com try/catch. Ao capturar uma Exception: grava um erro via ILogger e chama HandleExceptionAsync para serializar a resposta | Status 500 com Content-Type `application/problem+json`. Corpo: `ProblemDetails` com Title = "Erro interno no servidor", Detail = mensagem da exceção, Status = 500, Instance = caminho da requisição | Logs de erro são escritos pelo ILogger<GlobalExceptionMiddleware>. Útil para testes locais com a rota de exemplo `/api/throw-exception` que lança uma exceção propositalmente |

### Testes unitários

| Test class | Path | Objetivo |
|---|---:|---|
| MustContainUppercaseRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/MustContainUppercaseRuleTests.cs | Verifica regra "deve conter letra maiúscula". |
| MustContainLowercaseRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/MustContainLowercaseRuleTests.cs | Verifica regra "deve conter letra minúscula". |
| MustContainSpecialCharRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/MustContainSpecialCharRuleTests.cs | Verifica presença de pelo menos um caractere especial. |
| MustContainDigitRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/MustContainDigitRuleTests.cs | Verifica presença de dígitos (0-9). |
| MinLengthRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/MinLengthRuleTests.cs | Verifica requisito de comprimento mínimo. |
| NoRepeatedCharsRuleTests | ValidadorSenhaSegura.Tests/Domain/Rules/NoRepeatedCharsRuleTests.cs | Verifica se não há caracteres repetidos consecutivos. |
| WhitespaceNotAllowedTests | ValidadorSenhaSegura.Tests/Domain/Rules/WhitespaceNotAllowedTests.cs | Verifica que espaços em branco são proibidos. |
| NullNotAllowedTests | ValidadorSenhaSegura.Tests/Domain/Rules/NullNotAllowedTests.cs | Verifica comportamento quando valor nulo é recebido. |
| RulesetPasswordValidatorV1Tests | ValidadorSenhaSegura.Tests/Domain/Validators/RulesetPasswordValidatorV1Tests.cs | Testa o conjunto de regras da versão 1 do validador. |
| RulesetPasswordValidatorV2Tests | ValidadorSenhaSegura.Tests/Domain/Validators/RulesetPasswordValidatorV2Tests.cs | Testa o conjunto de regras da versão 2 do validador. |
| UseCasePasswordValidateTests | ValidadorSenhaSegura.Tests/Application/UseCases/UseCasePasswordValidateTests.cs | Testa seleção de validador por versão e comportamento do use-case. |
| ValidatorPipelineTests | ValidadorSenhaSegura.Tests/Shared/ValidatorPipelineTests.cs | Testa pipeline/execução encadeada das regras. |
| PasswordTests | ValidadorSenhaSegura.Tests/Domain/ValueObject/PasswordTests.cs | Testa ValueObject Password (construção, igualdade, etc.). |

### Testes integrados

| Test class | Path | Objetivo |
|---|---:|---|
| ValidatePasswordEndpointV1Tests | ValidadorSenhaSegura.Tests/Endpoints/ValidatePasswordEndpointV1Tests.cs | Integração do endpoint `/api/validate-password` usando `api-version: 1.0` (WebApplicationFactory). |
| ValidatePasswordEndpointV2Tests | ValidadorSenhaSegura.Tests/Endpoints/ValidatePasswordEndpointV2Tests.cs | Integração do endpoint `/api/validate-password` usando `api-version: 2.0` (WebApplicationFactory). |

-- fim das tabelas --

## Exemplos de Payloads e Respostas da API

Observações:
- A API é versionada por cabeçalho HTTP: `api-version: 1.0` ou `api-version: 2.0`.
- Os exemplos JSON seguem a serialização padrão do ASP.NET Core (camelCase).
- Endpoints principais:
  - POST /api/validate-password
  - GET  /api/hc
  - GET  /api/throw-exception (exemplo de erro interno)

  
1) Validar senha — requisição (exemplo curl, versão 1.0)
curl:
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 1.0" \
  -d '{ "password": "Senha123!" }'

Request body (JSON)
{
  "password": "Senha123!"
}

Resposta de sucesso (HTTP 200)
{
  "apiVersion": "1",
  "data": "A senha informada é válida",
  "errors": []
}

Resposta de falha (HTTP 400) — exemplo quando não atende às regras
{
  "apiVersion": "1",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    { "code": 1, "message": "A senha deve ter pelo menos 8 caracteres" },
    { "code": 4, "message": "Deve conter ao menos 1 caractere especial" }
  ]
}


2) Validar senha — usando versão 2.0 (pode ter regras diferentes conforme implementação)
curl:
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 2.0" \
  -d '{ "password": "abc" }'

Exemplo de resposta (HTTP 400)
{
  "apiVersion": "2",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    { "code": 1, "message": "A senha deve ter pelo menos 8 caracteres" },
    { "code": 2, "message": "Não são permitidos caracteres repetidos consecutivos" },
    { "code": 3, "message": "Espaços em branco não são permitidos" }
  ]
}


3) Health check (GET /api/hc)
curl:
curl -X GET "https://localhost:7218/api/hc" -H "api-version: 2.0"

Resposta (HTTP 200)
{
  "liveness": true,
  "readiness": true,
  "errors": []
}

4) Exemplo de erro interno tratado pelo middleware (GET /api/throw-exception)
curl:
curl -X GET "https://localhost:7218/api/throw-exception" -H "api-version: 1.0"

Resposta de erro (HTTP 500, content-type: application/problem+json)
{
  "title": "Erro interno no servidor",
  "detail": "Exception lançada na rota...",
  "status": 500,
  "instance": "/api/throw-exception"
}

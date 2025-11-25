# ValidadorSenhaSegura

API minimalista para validação de senhas robusta e escalável, construída com .NET 9 e C# 13. Projetada com separação de responsabilidades, versionamento por cabeçalho (api-version), arquitetura em camadas, tratamento centralizado de erros (RFC 7807) e uma suíte abrangente de testes.

---

## Índice

- [Visão geral](#visão-geral)
- [Destaques técnicos](#destaques-técnicos)
- [Pilares de boas práticas](#pilares-de-boas-práticas)
- [Como executar](#como-executar)
- [Arquitetura e organização](#arquitetura-e-organização)
- [Design Patterns — Racional, benefícios e trade-offs](#design-patterns---racional-benefícios-e-trade-offs)
- [Fluxo de validação](#fluxo-de-validação)
- [API — Endpoints, versões e exemplos](#api---endpoints-versões-e-exemplos)
- [Regras de validação e Value Object Password](#regras-de-validação-e-value-object-password)
- [Testes: estratégia e cobertura](#testes-estratégia-e-cobertura)
- [Observabilidade, segurança e performance](#observabilidade-segurança-e-performance)
- [Como estender (regras e versões)](#como-estender-regras-e-versões)
- [CI/CD, PR e checklist de qualidade](#cicd-pr-e-checklist-de-qualidade)
- [Arquivos e referências principais](#arquivos-e-referências-principais)
- [Contribuição](#contribuição)
- [Notas finais](#notas-finais)

---

## Visão geral

ValidadorSenhaSegura é um serviço que valida senhas baseadas em regras configuráveis. O projeto demonstra práticas de engenharia de software (DDD, SOLID, DRY, YAGNI), arquitetura em camadas, design patterns e foco em testabilidade e observabilidade.

Objetivos principais:
- Garantir invariantes do domínio (não existe Password inválido).
- Informar usuário com lista de erros (múltiplos problemas) e mensagens claras.
- Evolver via versionamento sem quebrar consumidores (backwards-compatible).
- Ser testável, extensível e preparado para produção.

---

## Destaques técnicos

- Minimal API (.NET 9) para menor overhead e boot mais rápido;
- Versionamento por header (api-version) — URLs limpas e gateway-friendly;
- Sem Controllers (Minimal API), endpoints declarados via rotas;
- Arquitetura em camadas (Application, Domain, Infrastructure, Shared);
- Regras isoladas (Specification Pattern) e Validator pipeline (Composite / CoR parcial);
- Value Object Password para garantia de invariantes;
- Middleware global de exceções com RFC 7807 (Problem Details);
- Swagger (OpenAPI) documentado por versão;
- Suíte de testes: Unitários + Testes de Integração (xUnit, Moq, WebApplicationFactory).

---

## Pilares de boas práticas

- Domain Driven Design (DDD): modelagem de domínio com Value Objects e regras expressas.
- SOLID: classes pequenas, responsabilidade única, aberturas para extensão (Open/Closed), dependências invertidas via DI.
- DRY: evitar duplicação de regras, reuso via specifications e rulesets.
- YAGNI: não implementar funcionalidades sem necessidade — arquitetura preparada para extensão.
- Testabilidade: regras testáveis isoladamente, orquestração testada, endpoints via testes integrados.
- Segurança & Privacidade: nunca exportar ou logar senhas; HTTPS em produção; recomendações de rate limiting no gateway.
- Observability: health checks, logging e tratamento consistente de erros.

---

## Como executar

Pré-requisitos
- .NET 9 SDK
- Visual Studio 2022 ou VS Code

CLI
```bash
git clone https://github.com/seu-usuario/ValidadorSenhaSegura.git
cd ValidadorSenhaSegura
dotnet build
dotnet run --project ValidadorSenhaSegura
# Swagger: https://localhost:7218/swagger
```

Executando testes
```bash
dotnet test
```

Executando com Docker (exemplo)
```bash
docker build -t validadorsenha .
docker run -p 7218:7218 validadorsenha
```

---

## Arquitetura e organização

Estrutura de pastas (resumida)
- Application: casos de uso, DTOs, validators, interações com Domain;
- Domain: regras, rulesets, value objects (Password), validators e interfaces;
- Infrastructure: integrações externas, configurações e infra;
- Shared: Result, ValidationResult, Error, Validator<T>, abstrações;
- Tests: unitários e integração.

Configuração central:
- Program.cs: DI, API versioning, middlewares e Swagger;
- Application/Configuration/RegisterModule.cs: composição de dependências;
- Application/Configuration/RegisterRoutes.cs: registro dos endpoints;
- Application/Configuration/RegisterMiddlewares.cs: pipeline de middlewares;
- Application/Middlewares/GlobalExceptionMiddleware.cs: RFC 7807.

---

## Design Patterns — Racional, benefícios e trade-offs

Abaixo, tabelas que auxiliam a entender porque cada padrão foi escolhido, seus benefícios e possíveis trade-offs.

### Tabela 1 — Padrões principais

| Padrão | Uso | Benefícios | Trade-offs | Alternativa |
|---|---|---|---|---|
| Strategy | Seleção de PasswordValidator (V1/V2) | Troca de algoritmo sem alterar orquestração; facilita versões | Aumenta nº de classes | Single Validator com flags (composición condicional) |
| Composite + Validator Pipeline | Validator<T> executando IValidationRule<T> | Acumula múltiplos erros; regras isoladas; alta extensibilidade | Ordem pode importar; gerir Estado em regras mais complexas | CoR clássico (Next) |
| Specification | IValidationRule<T> | Regras como objetos reusáveis e testáveis | Mais classes pequenas | Regras monolíticas |
| Value Object | Password VO | Invariantes garantidos; imutabilidade; igualdade por valor | Requer factory e Result wrapper | Strings nativas (sem invariantes) |
| Dependency Injection | RegisterModule | Testes e substituição de implementações | Sobredesgin se mal usado | Construção manual / instanciamento direto |

### Tabela 2 — Comparativo com alternativas e trade-offs práticos

| Decisão | Benefício prático | Perigo se não adotado |
|---|---|---|
| Versionamento por header | URLs limpas; compatível com gateways | Breaking changes em URLs; fragmentação de clients |
| Accumular erros (Composite) | UX: mensagens completas e acionáveis | Se não for tratado, volume de erros pode poluir logs |
| Value Object Password | Garante domínio válido em runtime | Strings expostas causam invariantes quebradas |
| GlobalExceptionMiddleware | Consistência de respostas e logs | Pode silenciar exceções se mal configurado |

---

## Fluxo de validação

1. O client faz POST /api/validate-password com header `api-version`.
2. UseCasePasswordValidate determina qual `IPasswordValidator` usar (Strategy).
3. Validator pipeline (Validator<T>) executa cada `IValidationRule<T>` e acumula erros (ValidationResult).
4. Se válido, será criado o Value Object `Password` através de Password.Create(...).
5. Se inválido, endpoint retorna 400 com ValidatePasswordResponse contendo lista de `Error`.
6. Exceções tratadas por GlobalExceptionMiddleware -> ProblemDetails (RFC 7807).

---

## API — Endpoints, versões e exemplos

Versionamento
- Header: `api-version: 1.0` ou `api-version: 2.0`;
- Default: 2.0 (Program.cs — AssumeDefaultVersionWhenUnspecified = true).

Endpoints principais
- POST /api/validate-password
  - Request: { "password": "Senha123!" }
  - Responses:
    - 200 OK — senha válida (ValidatePasswordResponse)
    - 400 Bad Request — invalidação com lista de erros
    - 500 Internal Server Error — ProblemDetails (RFC 7807)
- GET /api/hc — health check (liveness/readiness)
- GET /api/throw-exception — endpoint demo que força erro

Exemplo cURL v1.0 (sucesso)
```bash
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 1.0" \
  -d '{ "password": "Senha123!" }'
```
Exemplo v1.0 — response 200:
```json
{
  "apiVersion": "1",
  "data": "A senha informada é válida",
  "errors": []
}
```

Exemplo v2.0 (falha)
```bash
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 2.0" \
  -d '{ "password": "abc" }'
```
Exemplo v2.0 — response 400:
```json
{
  "apiVersion": "2",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    { "code": 1, "message": "A senha deve ter pelo menos 9 caracteres" },
    { "code": 2, "message": "Não são permitidos caracteres repetidos consecutivos" },
    { "code": 3, "message": "Espaços em branco não são permitidos" }
  ]
}
```

Health check (GET)
```bash
curl -X GET "https://localhost:7218/api/hc" -H "api-version: 2.0"
```
Response:
```json
{ "liveness": true, "readiness": true, "errors": [] }
```

Erro gerado (GET /api/throw-exception)
```bash
curl -X GET "https://localhost:7218/api/throw-exception" -H "api-version: 1.0"
```
Response 500 — ProblemDetails (RFC 7807)

---

## Regras de validação e Value Object Password

Regras (exemplos)
- MinLengthRule (mínimo de caracteres)
- MustContainUppercaseRule
- MustContainLowercaseRule
- MustContainDigitRule
- MustContainSpecialCharRule
- WhitespaceNotAllowedRule
- NoRepeatedCharsRule
- NullNotAllowedRule

Value Object — Password
- Imutável (record), equality by value;
- Factory method `Password.Create(password, IPasswordValidator)` valida antes de criar;
- Retorna `Result<Password>` com sucesso ou lista de erros (ValidationResult).

Exemplo de uso (pseudocódigo)
```csharp
var result = Password.Create("Senha123!", passwordValidator);
if (result.IsSuccess) {
    var password = result.Value;
} else {
    // errors => result.Errors
}
```

Motivação para VO
- Impõe invariantes do domínio;
- Evita leaky abstractions e uso indevido de strings;
- Facilita raciocínio, testes e migração para persistência (se necessário).

---

## Testes — estratégia e cobertura

Tipos de testes
- Unitários (Domain & Application).
- Integration/E2E (Endpoints com WebApplicationFactory).
- Mocks com Moq para dependências.

Cobertura de testes (exemplos)
- Domain/Rules/* — testes por regra;
- Domain/Validators/* — tests para RulesetPasswordValidatorV1/V2;
- Application/UseCases/UseCasePasswordValidateTests — testes de orquestração e Strategy selection;
- Endpoints/* — ValidatePasswordEndpointV1Tests, ValidatePasswordEndpointV2Tests (WebApplicationFactory).

Execução
```bash
dotnet test
```

Regras de ouro para testes
- Cada regra é testada isoladamente (AAA);
- Value Object garante invariantes e testabilidade sem DI;
- UseCase é testado com mocks (estratégia e diálogos com validators);
- Implementar testes de integração para garantir contratos (Swagger/DTOs).

---

## Observabilidade, segurança e performance (boas práticas)

Observability
- Logging estruturado (ILogger);
- Health-check endpoint;
- Tracing/telemetry sugerido (OpenTelemetry + Prometheus).

Segurança
- Não logar ou retornar a senha em responses/logs;
- HTTPS obrigatório em produção;
- Sugestão: rate-limiting e WAF em gateway;
- Validar tamanho máximo de payload para evitar abuso.

Performance
- Minimal APIs oferecem menor overhead;
- Regras puras são determinísticas e fáceis de paralelizar se necessário;
- Arquitetura stateless facilita scale horizontal.

---

## Como estender (regras e versões)

Adicionar regra
1. Implementar `IValidationRule<T>` com ValidationResult;
2. Incluir mensagem e código único de erro;
3. Adicionar rule no Ruleset (RulesetPasswordValidatorV1/V2 ou V3);
4. Registrar na DI (RegisterModule) se necessário;
5. Adicionar testes unitários e atualizar integração/Swagger.

Adicionar nova versão do validator (v3+)
1. Criar `PasswordValidatorV3` e `RulesetPasswordValidatorV3` com regras necessárias;
2. Registrar a strategy no `RegisterModule`;
3. Atualizar RegisterRoutes/Swagger (caso deseje visibilidade separada);
4. Garantir testes unitários e integrados para regressão.

Manter o YAGNI
- Implementar apenas o que for necessário; preferir extensões com testes e documentação.

---

## CI/CD, PR e checklist de qualidade (sugestão)

Pipeline sugerido:
- Dotnet build;
- Dotnet test (unitários e integração);
- Medir cobertura (coverlet/sonarqube);
- Lint e static analysis (Roslynator/Analyzers);
- Build image Docker e publicar (opcional).

Checklist de PR
- Build verde;
- Testes unitários e de integração passaram;
- Coverage >= meta;
- Descrição clara e rationale (breaking changes);
- Atualizar README/Swagger se for alteração de contrato.

---

## Arquivos e referências principais

- Program.cs — configuração geral de DI, API Versioning, Swagger e middlewares.
- Application/Configuration/RegisterModule.cs — composição e DI.
- Application/Configuration/RegisterRoutes.cs — endpoints e versionamento.
- Application/Configuration/RegisterMiddlewares.cs — pipeline de middlewares.
- Application/Middlewares/GlobalExceptionMiddleware.cs — RFC 7807.
- Application/UseCases/UseCasePasswordValidate.cs — estratégia de seleção.
- Application/Validators/Password/* — PasswordValidatorV1/PasswordValidatorV2.
- Domain/Validators/RulesetPasswordValidatorV1.cs / V2.cs — regras por versão.
- Domain/ValueObjects/Password.cs — Value Object.
- Shared/Abstractions/Validator.cs — Validator<T> pipeline.
- Shared/Result.cs, Shared/ValidationResult.cs, Shared/Error.cs — modelos de resposta.
- Http/* — exemplos de requisições (ValidatePassword.http, HealthCheck.http, ThrowException.http).

---

## Contribuição

Sugestões de melhoria
- Suportar regras configuráveis via JSON/YAML;
- Adicionar telemetria (OpenTelemetry) e dashboards;
- Adicionar rate-limiting e proteção via gateway.

---

## Notas finais

- O projeto prioriza qualidade e extensibilidade: regras isoladas, Value Objects e patterns que preservam invariantes e permitem extensão segura.

_Documentação consolidada para ValidadorSenhaSegura © 2025_
_Autor: Marcos Galhardi_
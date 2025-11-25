# ValidadorSenhaSegura

API minimalista para validação de senhas construída com .NET 9 (C# 13). Projetada com separação de responsabilidades, versionamento por cabeçalho, arquitetura em camadas e suíte abrangente de testes unitários e integrados.

---

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura & Design](#arquitetura--design)
3. [Princípios SOLID](#princípios-solid)
4. [Testes](#testes)
5. [Design de API](#design-de-api)
6. [Como Executar](#como-executar)
7. [Exemplos de Payloads](#exemplos-de-payloads-e-respostas-da-api)

---

## Visão Geral

### Destaques Técnicos

- **Minimal API (.NET 9)** — inicialização rápida, reduzindo cold start em ambientes serverless/containerizados
- **Versionamento por Cabeçalho HTTP** — suporta múltiplas versões no mesmo endpoint via `api-version: 1.0` ou `api-version: 2.0`, ideal para API Gateways
- **Sem Controllers** — endpoints registrados diretamente como Minimal APIs, reduzindo overhead desnecessário
- **Arquitetura em Camadas** — Application, Domain, Infrastructure e Shared, facilitando escalabilidade e manutenção
- **Validação Robusta com Regras Isoladas** — cada regra é uma classe testável seguindo Single Responsibility Principle
- **Middleware Global de Exceções** — centraliza tratamento de erros, retornando respostas padronizadas
- **Swagger/OpenAPI Versionado** — documentação automática separada por versão da API
- **Cobertura de Testes** — 13+ testes unitários e 2 testes integrados com xUnit + Moq

---

## Arquitetura & Design

### Estrutura em Camadas

- **Application**: Contém a lógica de aplicação e casos de uso.
- **Domain**: Contém entidades, agregados, repositórios e regras de negócio.
- **Infrastructure**: Implementações de repositórios, serviços externos e configurações.
- **Shared**: Contém código compartilhado entre camadas, como utilitários e classes base.

### Seleção de Validador por Estratégia

- **UseCasePasswordValidate**: Escolhe a implementação de `IPasswordValidator` baseada na versão da API usando o padrão Strategy.

### Modelo de Validação

- **ValidationResult + Error**: Permite retorno estruturado de erros.

### Middleware de Tratamento Global de Exceções

- **GlobalExceptionMiddleware**: Centraliza respostas de erro, capturando exceções não tratadas e retornando uma resposta padronizada.

### Endpoints de Health-Check e Throw-Exception

- **/api/hc**: Verifica a saúde da aplicação.
- **/api/throw-exception**: Demonstra o tratamento de exceções pelo middleware.

### Padrões de Design Utilizados

#### 1. **Strategy Pattern** — Múltiplas Versões do Validador
Cada versão da API (`V1_`, `V2_`) possui sua implementação de validador.

**Benefício:** Fácil adicionar novas versões sem modificar código existente (Open/Closed Principle).

#### 2. **Chain of Responsibility** — Pipeline de Regras
Classe `Validator<T>` aplica múltiplas regras sequencialmente:

**Benefício:** Adição de novas regras sem modificar a classe existente (Open/Closed Principle).

#### 3. **Dependency Injection** — Composição Root Centralizada
Todas as dependências registradas em `RegisterModule.cs`:

**Benefício:** Facilita testes (injeção de mocks), desacoplamento e flexibilidade.

#### 4. **Fluent Interface** — API Fluida e Expressiva

**Benefício:** Interface intuitiva, reduz verbosidade do código.

#### 5. **Value Object Pattern** — Classe Password
    ✔ Encapsula lógica de construção e validação de senhas (DDD).
        - Ex: 'Domain/ValueObject/Password.cs'

#### 6. **Tratamento Global de Erros** — Middleware `GlobalExceptionMiddleware` captura exceções não tratadas:
    ✔ Status 500 com `application/problem+json` (RFC 7807)
    ✔ Logs centralizados via `ILogger<GlobalExceptionMiddleware>`

#### 7. Design Patterns

    - Specification Pattern
        ✔ Cada uma das suas regras (IValidationRule<T>) é uma Specification.
            - Por quê, permite combinar regras facilmente e manter o código limpo.

    - Composite Pattern
        ✔ Validator<T> funciona como um Ruleset que compõe várias especificações.
            - Você monta uma coleção de regras.
            - Executa todas.
            - Coleta os erros.

    -  Chain of Responsibility (implícito)
        ✔ O Validator<T> processa cada regra em sequência.
            - Cada regra pode interromper a cadeia.
            - A pipeline termina dependendo de uma decisão própria da regra.
            - A execução flui através de uma sequência de handlers (regras).
            - Processa uma lista sequencial de handlers (regras).
            - Cada handler decide se:
                - Passa a requisição adiante, ou
                - Interrompe o fluxo.
            - O “encadeamento” não é explícito via propriedades Next, mas sim implícito dentro de uma lista iterada.

    - Value Object
        ✔ A classe Password é um Value Object, característico (DDD).

            Evidências claras:

                ✔ Tipo imutável (record + campo readonly)
                ✔ Só é criado através de um factory method (Create())
                ✔ Igualdade por valor (record)
                ✔ Encapsula validação dentro do processo de criação
                ✔ Representa um conceito do domínio: senha válida

        Declaração típica de Value Object

            🧩 Explicação do fluxo

                1. Password.Create() é chamado → Factory Method -> Password.Create(string, IPasswordValidator)

                2. O método chama a Strategy: → validator.Validate(password)

                3. A Password só é criada se a estratégia retornar SUCCESS

                    Senão, retorna: Result.Failure<Password>(errors)

                4. A classe Password é um Value Object

                    Imutável, criado apenas após a validação.
        
        Por que é um Value Object?

            - Não tem identidade própria (duas senhas iguais → mesmo valor).
            - É imutável.
            - Garantias invariantes (só existe se for válida).
            - Encapsula regras do domínio.
    
    - Factory Method

        ✔ O método estático Create(...) é um Factory Method, que:

            - Controla como o objeto é criado
            - Garante invariantes antes do objeto existir
            - Retorna um wrapper (Result<Password>) ao invés de exceção
        
            Evidências

                public static Result<Password> Create(string password, IPasswordValidator passwordValidator)

                Isso é muito usado em:

                    - DDD

                    - Clean Architecture

                    - Por que não usar new Password() diretamente?

                        Porque o domínio exige:

                            ❌ Não pode existir uma senha inválida.
                            ✔ A única forma de criar = Create() → com validação acoplada.


    - Strategy Pattern — via IPasswordValidator

        A dependência IPasswordValidator é uma Strategy, pois o algoritmo de validação NÃO está na classe Password, mas é injetado.

        Result<Password> Create(string password, passwordValidator)

        Isso significa:

            o algoritmo de validação é intercambiável

            você pode ter múltiplas estratégias de validação

                Exemplos:

                    PasswordValidatorV1
                    PasswordValidatorV2
                    "senha fraca" vs "senha forte"
                    regras configuráveis por JSON

        Evidências do Strategy Pattern:

            ✔ interface com método comum (Validate)
            ✔ comportamento externo injetado
            ✔ fixar invariantes sem fixar a implementação da regra


#### 10. **Swagger Automático** — Documentação gerada por versão

---

## Princípios SOLID

| Princípio | Implementação | Benefício |
|-----------|---------------|-----------|
| **S**ingle Responsibility | Cada regra (`MinLengthRule`, `MustContainDigitRule`, etc.) valida **um** aspecto | Fácil manutenção, testes isolados |
| **O**pen/Closed | Adicione regras sem modificar `Validator<T>`; adicione validadores sem tocar em `UseCasePasswordValidate` | Extensível a novos requisitos |
| **L**iskov Substitution | Todas as regras implementam `IValidationRule<T>` com mesmo contrato | Polimorfismo seguro |
| **I**nterface Segregation | Interfaces pequenas e focadas: `IPasswordValidator`, `IValidationRule<T>`, `IUseCasePasswordValidate` | Desacoplamento, sem herança desnecessária |
| **D**ependency Inversion | Depende de abstrações (`IPasswordValidator`), não de implementações concretas | Inversão de controle, facilita testes com mocks |

### Exemplo de Extensibilidade (Novo Validador v3)

---

## Testes

### Estratégia de Testes

A suíte combina **testes unitários** (domínio/lógica isolada) com **testes integrados** (fluxo end-to-end):

#### Cobertura por Camada

| Camada | Estratégia | Ferramentas |
|--------|-----------|-------------|
| **Domain** | Testes unitários isolados (sem DI) | xUnit, Theory [InlineData] |
| **Application** | Testes unitários com mocks | xUnit, Moq |
| **Endpoints** | Testes integrados (WebApplicationFactory) | xUnit, Microsoft.AspNetCore.Mvc.Testing |

### Testes Unitários

**Objetivo:** Validar regras de negócio isoladamente, sem contexto HTTP.

| Classe de Teste | Arquivo | Cenários |
|---|---|---|
| `MustContainUppercaseRuleTests` | `Domain/Rules/MustContainUppercaseRuleTests.cs` | ✓ Contém maiúscula / ✗ Não contém |
| `MustContainLowercaseRuleTests` | `Domain/Rules/MustContainLowercaseRuleTests.cs` | ✓ Contém minúscula / ✗ Não contém |
| `MustContainDigitRuleTests` | `Domain/Rules/MustContainDigitRuleTests.cs` | ✓ Contém dígito / ✗ Não contém |
| `MustContainSpecialCharRuleTests` | `Domain/Rules/MustContainSpecialCharRuleTests.cs` | ✓ Contém especial / ✗ Não contém |
| `MinLengthRuleTests` | `Domain/Rules/MinLengthRuleTests.cs` | ✓ Mín. atendido / ✗ Mín. não atendido |
| `NoRepeatedCharsRuleTests` | `Domain/Rules/NoRepeatedCharsRuleTests.cs` | ✓ Sem repetição / ✗ Com repetição |
| `WhitespaceNotAllowedTests` | `Domain/Rules/WhitespaceNotAllowedTests.cs` | ✓ Sem espaço / ✗ Com espaço |
| `NullNotAllowedTests` | `Domain/Rules/NullNotAllowedTests.cs` | ✓ Válido / ✗ Nulo |
| `RulesetPasswordValidatorV1Tests` | `Domain/Validators/RulesetPasswordValidatorV1Tests.cs` | Orquestração de regras V1 |
| `RulesetPasswordValidatorV2Tests` | `Domain/Validators/RulesetPasswordValidatorV2Tests.cs` | Orquestração de regras V2 |
| `ValidatorPipelineTests` | `Shared/ValidatorPipelineTests.cs` | Chain of Responsibility |
| `PasswordTests` | `Domain/ValueObject/PasswordTests.cs` | Value Object: igualdade, construção |
| `UseCasePasswordValidateTests` | `Application/UseCases/UseCasePasswordValidateTests.cs` | Strategy, seleção correta de validador |

**Exemplo de Teste (AAA Pattern):**

### Testes Integrados

**Objetivo:** Validar fluxo HTTP completo: request → middleware → endpoint → response.

| Classe de Teste | Arquivo | Cobertura |
|---|---|---|
| `ValidatePasswordEndpointV1Tests` | `Endpoints/ValidatePasswordEndpointV1Tests.cs` | POST /api/validate-password com api-version: 1.0 |
| `ValidatePasswordEndpointV2Tests` | `Endpoints/ValidatePasswordEndpointV2Tests.cs` | POST /api/validate-password com api-version: 2.0 |

**Exemplo de Teste Integrado:**

---

## Design de API

### Princípios de Design

1. **Versionamento via Cabeçalho** — Mantém URLs limpas, ideal para Gateway
2. **Resposta Padronizada** — Sempre segue o mesmo formato:

---

## Exemplos de Payloads e Respostas da API

Observações:
- A API é versionada por cabeçalho HTTP: `api-version: 1.0` ou `api-version: 2.0`.
- Os exemplos JSON seguem a serialização padrão do ASP.NET Core (camelCase).
- Endpoints principais:
  - POST /api/validate-password
  - GET  /api/hc
  - GET  /api/throw-exception (exemplo de erro interno)

### 1. Validar Senha — Versão 1.0 (Sucesso)

**Request:**
curl:
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 1.0" \
  -d '{ "password": "Senha123!" }'

Request body (JSON)
{
  "password": "Senha123!"
}

**Response (HTTP 200):**
{
  "apiVersion": "1",
  "data": "A senha informada é válida",
  "errors": []
}

**Response (HTTP 400):**
{
  "apiVersion": "1",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    { "code": 1, "message": "A senha deve ter pelo menos 9 caracteres" },
    { "code": 4, "message": "Deve conter ao menos 1 caractere especial" }
  ]
}

### 2. Validar Senha — Versão 2.0 (Falha)

**Request:**
curl:
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 2.0" \
  -d '{ "password": "abc" }'

**Response:**
Exemplo de resposta (HTTP 400)
{
  "apiVersion": "2",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    { "code": 1, "message": "A senha deve ter pelo menos 9 caracteres" },
    { "code": 2, "message": "Não são permitidos caracteres repetidos consecutivos" },
    { "code": 3, "message": "Espaços em branco não são permitidos" }
  ]
}

### 3. Health Check

**Request:**
curl:
curl -X GET "https://localhost:7218/api/hc" -H "api-version: 2.0"

**Response:**
Resposta (HTTP 200)
{
  "liveness": true,
  "readiness": true,
  "errors": []
}

### 4. Erro Não Tratado (Middleware)

**Request:**
curl:
curl -X GET "https://localhost:7218/api/throw-exception" -H "api-version: 1.0"

**Response:**
Resposta de erro (HTTP 500, content-type: application/problem+json)
{
  "title": "Erro interno no servidor",
  "detail": "Exception lançada na rota...",
  "status": 500,
  "instance": "/api/throw-exception"
}

**Response (HTTP 500, RFC 7807):**

---

## Como Executar

### Pré-requisitos

- .NET 9 SDK
- Visual Studio 2022 ou VS Code

### Build

- **Build**: use __Build Solution__ no Visual Studio ou `dotnet build` na raiz da solução.
- **Run**: use __Start Debugging__ / __Run__ ou `dotnet run --project ValidadorSenhaSegura`.
- **Testes**: use __Test Explorer__ ou `dotnet test` na raiz.

### Detalhes de Implementação Notáveis

- **Versionamento**: Implementado para demonstrar a resolução técnica de versionamento por cabeçalho.
- **Versão Padrão**: Configurada como 2.0 com `AssumeDefaultVersionWhenUnspecified = true`.
- **Swagger**: Configurado para emitir um documento por versão usando `IApiVersionDescriptionProvider`.
- **Regras de Validação**: Classes pequenas e testáveis (MinLengthRule, MustContainDigitRule, etc.).
- **RulesetPasswordValidator**: Aplica mínimo de caracteres, sem repetição de caracteres, sem espaços e requisitos básicos via regex.

---

### Execução

A API estará disponível em `https://localhost:7218`.

---

## Middleware Global

| Middleware | Objetivo | Comportamento | Response |
|---|---|---|---|
| **GlobalExceptionMiddleware** | Capturar exceções não tratadas | Envolve chamada ao próximo middleware com try/catch; serializa erro como `ProblemDetails` | HTTP 500 com `application/problem+json` |

---

## Notas de Design

### Por que Minimal API?
- Reduz overhead de controllers
- Melhor cold start em serverless
- Ideal para microserviços pequenos

### Por que Versionamento por Cabeçalho?
- URL fica limpa
- Compatível com API Gateways
- Fácil de adicionar novas versões

### Por que Regras Isoladas?
- Cada regra é testável isoladamente
- Fácil reusar regras em diferentes validadores
- Novo requisito = nova regra, sem modificar código existente

### Por que Value Objects?
- Encapsula lógica de construção
- Imutabilidade padrão (record)
- Facilita Domain-Driven Design

### Por que Chain of Responsability (parcial)?
    
    Segue abaixo uma análise das vantagens e desvantagens da implementação atual do padrão Chain of Responsibility (CoR parcial) na validação de senhas.
    
    👍 Vantagens

        ✔ mais simples de implementar
        ✔ regras independentes
        ✔ fácil adicionar/remover regras (AddRule)
        ✔ fácil criar lista dinâmica de regras
        ✔ funciona muito bem com DI (injeção de múltiplos handlers)
        ✔ pipeline centralizado no Validator
        ✔ permite acumular múltiplos erros
        ✔ permitir "parada precoce" via ContinueIfErrorOccurs sem complicar

    👎 Desvantagens

        ❌ a lógica de fluxo não fica nas regras — fica no Validator
        ❌ não é um Chain of Responsibility “puro”
        ❌ regras não sabem qual é a próxima na cadeia
        ❌ não há composição hierárquica de regras (ex: OR, AND, XOR de regras)

    
    Segue abaixo uma análise das vantagens e desvantagens da implementação atual do padrão Chain of Responsibility (CoR clássico) na validação de senhas, modelo CoR Clássico (cada rule tem um Next)
    
    👍 Vantagens

        ✔ implementação 100% alinhada ao padrão CoR clássico
        ✔ cada regra controla seu próprio fluxo
        ✔ flexível para montar árvores de regras (regra X chama regra Y)
        ✔ as regras conhecem a sequência e têm controle total
        ✔ muito útil quando você precisa de fluxos dinâmicos complexos

    👎 Desvantagens

        ❌ mais código
        ❌ mais difícil adicionar/remover regras dinamicamente
        ❌ regras ficam acopladas ao fluxo (chamam explícitamente o próximo)
        ❌ mais difícil acumular todas as mensagens de erro (CoR é naturalmente "short circuit")
        ❌ um CoR bem feito normalmente retorna um único erro — não vários

    Escolha do Modelo Atual:

        Motivos:

            - Suporta múltiplos erros
            - Fácil adicionar regras
            - Fácil configurar ruleset por arquivo (JSON, YAML etc.)
            - Regras são super isoladas e plugáveis
            - O fluxo é simples
            - O CoR clássico só é melhor em casos de:
            - Fluxo condicional complexo
            - Pipelines ramificadas
            - Tarefas que devem ser passadas de handler para handler
            - Não validação de regras paralelas
            - Para "validação de senha", usar Chain of Responsibility clássico complica sem oferecer ganho efetivo.

--------

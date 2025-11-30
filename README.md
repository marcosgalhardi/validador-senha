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
- [✅ DECISÕES DE DESIGN - ESPECÍFICAS DO DESAFIO](#decisões-de-design---específicas-do-desafio)

---

## Visão geral

ValidadorSenhaSegura é um serviço que valida senhas baseadas em regras configuráveis. O projeto demonstra práticas de engenharia de software (DDD, SOLID, DRY, YAGNI), arquitetura em camadas, design patterns e foco em testabilidade e observabilidade.

**Objetivos principais:**
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

- **Domain Driven Design (DDD)**: modelagem de domínio com Value Objects e regras expressas.
- **SOLID**: classes pequenas, responsabilidade única, aberturas para extensão (Open/Closed), dependências invertidas via DI.
- **DRY**: evitar duplicação de regras, reuso via specifications e rulesets.
- **YAGNI**: não implementar funcionalidades sem necessidade — arquitetura preparada para extensão.
- **Testabilidade**: regras testáveis isoladamente, orquestração testada, endpoints via testes integrados.
- **Segurança & Privacidade**: nunca exportar ou logar senhas; HTTPS em produção; recomendações de rate limiting no gateway.
- **Observability**: health checks, logging e tratamento consistente de erros.

---

## Como executar

**Pré-requisitos**
- .NET 9 SDK
- Visual Studio 2022 ou VS Code

**CLI**
```bash
git clone https://github.com/marcosgalhardi/validador-senha.git
cd validador-senha
dotnet build
dotnet run --project ValidadorSenhaSegura
# Swagger: https://localhost:7218/swagger
```

**Executando testes**
```bash
dotnet test
```

---

## API — Endpoints, versões e exemplos

**Versionamento**
- Header: `api-version: 1.0` ou `api-version: 2.0`
- Default: 2.0

**Endpoint principal**
```
POST /api/validate-password
```

**Request**
```json
{
  "password": "AbTp9!fok"
}
```

**Response 200 (válida)**
```json
{
  "apiVersion": "2",
  "data": "A senha informada é válida",
  "errors": []
}
```

**Response 400 (inválida)**
```json
{
  "apiVersion": "2",
  "data": "A senha informada é inválida, pois não atende aos critérios",
  "errors": [
    {
      "code": 1,
      "message": "A senha deve ter pelo menos 9 caracteres"
    }
  ]
}
```

**cURL exemplo (válida)**
```bash
curl -X POST "https://localhost:7218/api/validate-password" \
  -H "Content-Type: application/json" \
  -H "api-version: 2.0" \
  -d '{ "password": "AbTp9!fok" }'
```

**Health check**
```bash
curl -X GET "https://localhost:7218/api/hc" -H "api-version: 2.0"
```

---

## ✅ DECISÕES DE DESIGN - ESPECÍFICAS DO DESAFIO

| Requisito do desafio | Implementação | Racional |
|---|---|---|
| **9+ caracteres** | `MinLengthRule` | Validação explícita no pipeline |
| **1 dígito** | `MustContainDigitRule` | Regra isolada, testável |
| **1 minúscula** | `MustContainLowercaseRule` | |
| **1 maiúscula** | `MustContainUppercaseRule` | |
| **1 especial (!@#$%^&*()-+)** | `MustContainSpecialCharRule` | Lista exata dos caracteres permitidos |
| **Sem caracteres repetidos** | `NoRepeatedCharsRule` | Verifica duplicatas no conjunto |
| **Espaços inválidos** | **✅ MELHORIA**: `password.Replace(" ", string.Empty)` antes da validação | **Alinha com 'não devem ser considerados válidos'** |

**Tratamento de espaços**: Removidos antes da validação para tratar `AbTp9 fok` → `AbTp9fok` (válida conforme exemplo).

---

## Testes: estratégia e cobertura

**Cobertura dos exemplos do desafio**:

| Exemplo | Esperado | Teste implementado |
|---|---|---|
| `""` | false | ✅ `EmptyPasswordTest` |
| `"aa"` | false | ✅ `RepeatedCharsTest` |
| `"ab"` | false | ✅ `MinLengthTest` |
| `"AAAbbbCc"` | false | ✅ `NoDigitTest` |
| `"AbTp9!foo"` | false | ✅ `RepeatedCharO` |
| `"AbTp9!foA` | false | ✅ `RepeatedCharA` |
| `"AbTp9 fok"` | true | ✅ `SpacesRemovedBecomesValid` |
| `"AbTp9!fok"` | true | ✅ `AllRulesSatisfied` |

**Casos de borda adicionais**:
- Apenas caracteres especiais
- Senha com espaço no meio
- Máximo tamanho

---

**_Documentação consolidada para ValidadorSenhaSegura © 2025_**
**_Autor: Marcos Galhardi_**
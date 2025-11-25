using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators;

namespace ValidadorSenhaSegura.Tests.Validators
{
    public class RulesetPasswordValidatorV1Tests
    {
        private readonly RulesetPasswordValidatorV1 _validator;

        public RulesetPasswordValidatorV1Tests()
        {
            _validator = new RulesetPasswordValidatorV1();
        }

        [Fact]
        public void Validate_ShouldReturnErrors_WhenPasswordIsNull()
        {
            var result = _validator.Validate(null!);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.MinLengthRuleValidation);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.NoRepeatedCharsRuleValidation);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.WhitespaceNotAllowed);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.MinimumRequirementsRuleValidation);
        }

        [Fact]
        public void Validate_ShouldReturnError_WhenPasswordHasRepeatedCharacters()
        {
            var password = "Aa1!aaaaa"; // contém caracteres repetidos

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.NoRepeatedCharsRuleValidation);
        }

        [Fact]
        public void Validate_ShouldReturnError_WhenPasswordHasWhitespace()
        {
            var password = "Aa1! aa11";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.WhitespaceNotAllowed);
        }

        [Fact]
        public void Validate_ShouldReturnError_WhenRegexRequirementsFail()
        {
            var password = "Aaaaaaaa!"; // sem número

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == (int)RulesetValidationErrorCode.MinimumRequirementsRuleValidation);
        }

        [Fact]
        public void Validate_ShouldBeValid_WhenPasswordIsStrongAndUniqueCharacters()
        {
            var password = "Ab1!Cd2@E"; // atende todos os requisitos

            var result = _validator.Validate(password);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenPasswordHasMultipleViolations()
        {
            var password = "aa aa"; // curto, repetido, espaço, não passa regex

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Equal(4, result.Errors.Count); // todas as regras quebradas
        }
    }
}

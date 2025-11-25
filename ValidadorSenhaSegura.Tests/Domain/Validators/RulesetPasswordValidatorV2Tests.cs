using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Password;

namespace ValidadorSenhaSegura.Tests.Validators
{
    public class RulesetPasswordValidatorV2Tests
    {
        private readonly RulesetPasswordValidatorV2 _validator;

        public RulesetPasswordValidatorV2Tests()
        {
            _validator = new RulesetPasswordValidatorV2();
        }

        [Fact]
        public void ApiVersion_ShouldBeV2()
        {
            Assert.Equal(ApiVersion.V2_, _validator.ApiVersion);
        }

        [Fact]
        public void Validate_ShouldReturnSuccess_ForValidPassword()
        {
            // password atende todos os critérios
            var password = "Abc1!Def2";

            var result = _validator.Validate(password);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_ShouldFail_WhenPasswordIsNull()
        {
            var result = _validator.Validate(null!);

            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void Validate_ShouldFail_WhenContainsWhitespace()
        {
            var password = "Aa1! aaaA";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.WhitespaceNotAllowed);
        }

        [Fact]
        public void Validate_ShouldFail_WhenTooShort()
        {
            var password = "Aa1!a";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.MinLengthRuleValidation);
        }

        [Fact]
        public void Validate_ShouldFail_WhenMissingDigit()
        {
            var password = "Abc!DefGh";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.MustContainDigitRuleValidation);
        }

        [Fact]
        public void Validate_ShouldFail_WhenMissingLowercase()
        {
            var password = "ABC1!DEF2";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.MustContainLowercaseRuleValidation);
        }

        [Fact]
        public void Validate_ShouldFail_WhenMissingUppercase()
        {
            var password = "abc1!def2";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.MustContainUppercaseRuleValidation);
        }

        [Fact]
        public void Validate_ShouldFail_WhenMissingSpecialChar()
        {
            var password = "Abc1Def23";

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.MustContainSpecialCharRuleValidation);
        }

        [Fact]
        public void Validate_ShouldFail_WhenRepeatedCharacters()
        {
            var password = "Aa1!aaaaa"; // contém repetidos

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors,
                e => e.Code == (int)RulesValidationErrorCode.NoRepeatedCharsRuleValidation);
        }

        [Fact]
        public void Validate_ShouldReturnAllErrors_WhenMultipleRulesFail()
        {
            var password = "aaaaaa"; // min length, no uppercase, no digit, no special, repeated chars

            var result = _validator.Validate(password);

            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count >= 5);
        }
    }
}

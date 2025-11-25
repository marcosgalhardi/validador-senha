using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class MustContainDigitRuleTests
    {
        private readonly MustContainDigitRule _rule;

        public MustContainDigitRuleTests()
        {
            _rule = new MustContainDigitRule();
        }

        [Theory]
        [InlineData("abc123")]
        [InlineData("1abcdef")]
        [InlineData("abc1def")]
        [InlineData("0")]
        [InlineData("SenhaCom9")]
        public void IsValid_ShouldReturnTrue_WhenStringContainsAtLeastOneDigit(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abcdef")]
        [InlineData("SenhaSemNumero")]
        [InlineData("!@#$%¨&*()")]
        public void IsValid_ShouldReturnFalse_WhenStringDoesNotContainDigit(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ErrorMessage_ShouldBeCorrect()
        {
            // Assert
            Assert.Equal("Deve ter ao menos um número.", _rule.ErrorMessage);
        }

        [Fact]
        public void ErrorCode_ShouldBeCorrect()
        {
            // Assert
            Assert.Equal(RulesValidationErrorCode.MustContainDigitRuleValidation, _rule.ErrorCode);
        }
    }
}

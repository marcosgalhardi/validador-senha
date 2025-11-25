using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class MustContainLowercaseRuleTests
    {
        private readonly MustContainLowercaseRule _rule;

        public MustContainLowercaseRuleTests()
        {
            _rule = new MustContainLowercaseRule();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("abc")]
        [InlineData("ABCdEF")]
        [InlineData("123a456")]
        [InlineData("SenhaComMinuscula")]
        public void IsValid_ShouldReturnTrue_WhenContainsLowercaseLetter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ABC")]
        [InlineData("123456")]
        [InlineData("!@#$%¨&*()")]
        [InlineData("SENHATODASMAIUSCULAS")]
        public void IsValid_ShouldReturnFalse_WhenDoesNotContainLowercaseLetter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ErrorMessage_ShouldMatchExpectedMessage()
        {
            // Assert
            Assert.Equal("Deve ter letra minúscula.", _rule.ErrorMessage);
        }

        [Fact]
        public void ErrorCode_ShouldMatchExpectedErrorCode()
        {
            // Assert
            Assert.Equal(RulesValidationErrorCode.MustContainLowercaseRuleValidation, _rule.ErrorCode);
        }
    }
}

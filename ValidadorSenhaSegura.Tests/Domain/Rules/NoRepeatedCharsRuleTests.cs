using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class NoRepeatedCharsRuleTests
    {
        private readonly NoRepeatedCharsRule _rule = new();

        [Theory]
        [InlineData("abc")]
        [InlineData("Abc123")]
        [InlineData("!@#$%")]
        [InlineData("A1b2C3")]
        public void IsValid_ShouldReturnTrue_WhenNoRepeatedCharacters(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("aa")]
        [InlineData("abca")]
        [InlineData("1123")]
        [InlineData("AAbb")]
        [InlineData("!!ab")]
        public void IsValid_ShouldReturnFalse_WhenHasRepeatedCharacters(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ErrorMessage_ShouldBeCorrect()
        {
            Assert.Equal("Não pode ter caracteres repetidos.", _rule.ErrorMessage);
        }

        [Fact]
        public void ErrorCode_ShouldBeCorrect()
        {
            Assert.Equal(RulesValidationErrorCode.NoRepeatedCharsRuleValidation, _rule.ErrorCode);
        }
    }
}

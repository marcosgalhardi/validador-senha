using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class WhitespaceNotAllowedTests
    {
        private readonly WhitespaceNotAllowed _rule = new();

        [Theory]
        [InlineData("abc")]
        [InlineData("Senha123!")]
        [InlineData("ABC_def")]
        [InlineData("!@#$%ˆ&*()")]
        public void IsValid_ShouldReturnTrue_WhenNoWhitespace(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("a bc")]
        [InlineData(" senha")]
        [InlineData("senha ")]
        [InlineData("a b c")]
        [InlineData(" ")]
        public void IsValid_ShouldReturnFalse_WhenWhitespaceIsPresent(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ErrorMessage_ShouldBeCorrect()
        {
            Assert.Equal("Espaço em branco é um caractere inválido.", _rule.ErrorMessage);
        }

        [Fact]
        public void ErrorCode_ShouldBeCorrect()
        {
            Assert.Equal(RulesValidationErrorCode.WhitespaceNotAllowed, _rule.ErrorCode);
        }
    }
}

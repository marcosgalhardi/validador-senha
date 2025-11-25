using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class MustContainUppercaseRuleTests
    {
        private readonly MustContainUppercaseRule _rule;

        public MustContainUppercaseRuleTests()
        {
            _rule = new MustContainUppercaseRule();
        }

        // ----------------------------------------------------------
        // TESTES PARA VALORES VÁLIDOS (possuem letra maiúscula)
        // ----------------------------------------------------------
        [Theory]
        [InlineData("A")]
        [InlineData("Abc")]
        [InlineData("abcD")]
        [InlineData("senhaComMaiusculaX")]
        [InlineData("123Z456")]
        [InlineData("!@#A!@#")]
        public void IsValid_ShouldReturnTrue_WhenContainsUppercaseLetter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        // ----------------------------------------------------------
        // TESTES PARA VALORES INVÁLIDOS (sem letra maiúscula)
        // ----------------------------------------------------------
        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        [InlineData("123")]
        [InlineData("senha minuscula")]
        [InlineData("çáéíóú")]
        [InlineData("!@#$%")]
        public void IsValid_ShouldReturnFalse_WhenDoesNotContainUppercaseLetter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        // ----------------------------------------------------------
        // VALIDA A MENSAGEM DE ERRO
        // ----------------------------------------------------------
        [Fact]
        public void ErrorMessage_ShouldBeCorrect()
        {
            Assert.Equal("Deve ter letra maiúscula.", _rule.ErrorMessage);
        }

        // ----------------------------------------------------------
        // VALIDA O CÓDIGO DE ERRO
        // ----------------------------------------------------------
        [Fact]
        public void ErrorCode_ShouldBeCorrect()
        {
            Assert.Equal(RulesValidationErrorCode.MustContainUppercaseRuleValidation, _rule.ErrorCode);
        }
    }
}

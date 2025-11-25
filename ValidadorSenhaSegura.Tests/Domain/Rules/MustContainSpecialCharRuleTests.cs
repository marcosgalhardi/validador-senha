using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Rules;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class MustContainSpecialCharRuleTests
    {
        private readonly MustContainSpecialCharRule _rule;

        public MustContainSpecialCharRuleTests()
        {
            _rule = new MustContainSpecialCharRule();
        }

        // ----------------------------------------------------------
        // TESTES DE CASOS VÁLIDOS (pelo menos um caractere especial)
        // ----------------------------------------------------------
        [Theory]
        [InlineData("senha!")]
        [InlineData("@inicio")]
        [InlineData("meio#texto")]
        [InlineData("final$")]
        [InlineData("multi%^&*")]
        [InlineData("()-+")]
        public void IsValid_ShouldReturnTrue_WhenContainsSpecialCharacter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.True(result);
        }

        // ----------------------------------------------------------
        // TESTES DE CASOS INVÁLIDOS (nenhum caractere especial)
        // ----------------------------------------------------------
        [Theory]
        [InlineData("")]
        [InlineData("senha")]
        [InlineData("SENHA123")]
        [InlineData("somente letras")]
        [InlineData("123456")]
        [InlineData("abcDEF123")]
        public void IsValid_ShouldReturnFalse_WhenDoesNotContainSpecialCharacter(string input)
        {
            // Act
            var result = _rule.IsValid(input);

            // Assert
            Assert.False(result);
        }

        // ----------------------------------------------------------
        // VALIDANDO MENSAGEM DE ERRO
        // ----------------------------------------------------------
        [Fact]
        public void ErrorMessage_ShouldBeCorrect()
        {
            Assert.Equal("Deve ter caractere especial.", _rule.ErrorMessage);
        }

        // ----------------------------------------------------------
        // VALIDANDO CÓDIGO DE ERRO
        // ----------------------------------------------------------
        [Fact]
        public void ErrorCode_ShouldBeCorrect()
        {
            Assert.Equal(RulesValidationErrorCode.MustContainSpecialCharRuleValidation, _rule.ErrorCode);
        }
    }
}

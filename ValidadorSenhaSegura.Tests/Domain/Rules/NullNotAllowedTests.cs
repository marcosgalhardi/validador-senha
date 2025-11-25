using ValidadorSenhaSegura.Domain.Rules;
using ValidadorSenhaSegura.Domain.Enums;

namespace ValidadorSenhaSegura.Tests.Domain.Rules
{
    public class NullNotAllowedTests
    {
        private readonly NullNotAllowed _rule = new();

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenInputIsNull()
        {
            Assert.False(_rule.IsValid(null!));
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenInputIsNotNull()
        {
            Assert.True(_rule.IsValid("any"));
        }

        [Fact]
        public void Properties_ShouldExposeExpectedValues()
        {
            Assert.Equal("Nulo não é permitido.", _rule.ErrorMessage);
            Assert.Equal(RulesValidationErrorCode.NullNotAllowed, _rule.ErrorCode);
            Assert.False(_rule.ContinueIfErrorOccurs);
        }
    }
}
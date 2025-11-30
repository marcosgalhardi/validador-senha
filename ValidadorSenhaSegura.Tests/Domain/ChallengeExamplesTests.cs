using Xunit;
using ValidadorSenhaSegura.Domain.ValueObjects;
using ValidadorSenhaSegura.Domain.Enums;
using ValidadorSenhaSegura.Domain.Validators.Interfaces;
using ValidadorSenhaSegura.Domain.Validators.Rules;
using ValidadorSenhaSegura.Shared;
using Moq;

namespace ValidadorSenhaSegura.Tests.Domain;

public class PasswordValidationTests
{
    private readonly Mock<IPasswordValidator> _mockValidator;
    private readonly IEnumerable<IValidationRule<string>> _rules;

    public PasswordValidationTests()
    {
        _mockValidator = new Mock<IPasswordValidator>();
        _rules = new List<IValidationRule<string>>();
    }

    [Theory]
    [InlineData("", false, "senha vazia")]
    [InlineData("aa", false, "caracteres repetidos")]
    [InlineData("ab", false, "tamanho insuficiente")]
    [InlineData("AAAbbbCc", false, "sem dígito")]
    [InlineData("AbTp9!foo", false, "caractere 'o' repetido")]
    [InlineData("AbTp9!foA", false, "caractere 'A' repetido")]
    [InlineData("AbTp9 fok", true, "espaços removidos → válida")]
    [InlineData("AbTp9!fok", true, "todos critérios atendidos")]
    public void ValidatePassword_AllChallengeExamples(string inputPassword, bool expectedValid, string description)
    {
        // Arrange
        _mockValidator.Setup(v => v.ApiVersion).Returns(ApiVersion.V1);
        var passwordValidator = _mockValidator.Object;

        // Act - espaços já removidos no UseCase
        var result = Password.Create(inputPassword.Replace(" ", ""), passwordValidator);

        // Assert
        Assert.Equal(expectedValid, result.IsSuccess);
        Assert.NotNull(result.Errors);
    }

    [Fact]
    public void ValidatePassword_OnlySpecialChars_Valid()
    {
        var result = Password.Create("!@#$%^&*", _mockValidator.Object);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ValidatePassword_OnlyNumbers_Invalid()
    {
        var result = Password.Create("123456789", _mockValidator.Object);
        Assert.False(result.IsSuccess);
    }
}
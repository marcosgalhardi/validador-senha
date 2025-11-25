using ValidadorSenhaSegura.Application.Validators.Password;
using ValidadorSenhaSegura.Domain.Validators;
using ValidadorSenhaSegura.Domain.Validators.Password;
using ValidadorSenhaSegura.Domain.ValueObject;

public class PasswordTests
{
    [Fact]
    public void ShouldCreateValidPasswordV1()
    {
        var password = Password.Create("Abcde123!", 
            new PasswordValidatorV1(
                [new RulesetPasswordValidatorV1(), 
                new RulesetPasswordValidatorV2()
            ])
        );

        Assert.True(password.IsSuccess);
    }

    [Fact]
    public void ShouldCreateValidPasswordV2()
    {
        var password = Password.Create("Abcde123!", 
            new PasswordValidatorV2([
                new RulesetPasswordValidatorV1(), 
                new RulesetPasswordValidatorV2()
            ])
        );

        Assert.True(password.IsSuccess);
    }
}

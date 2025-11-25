using ValidadorSenhaSegura.Domain.Rules;

public class MinLengthRuleTests
{
    [Theory]
    [InlineData(9, "12345678", false)]
    [InlineData(9, "123456789", true)]
    public void ShouldValidateMinLengthRule(int minLength, string password, bool expected)
    {
        var rule = new MinLengthRule(minLength);
        var result = rule.IsValid(password);

        Assert.Equal(expected, result);
    }
}

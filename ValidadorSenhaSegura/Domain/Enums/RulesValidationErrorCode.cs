namespace ValidadorSenhaSegura.Domain.Enums
{
    public enum RulesValidationErrorCode
    {
        NotSet = -1,
        MinLengthRuleValidation = 1,
        MustContainDigitRuleValidation = 2,
        MustContainLowercaseRuleValidation = 3,
        MustContainSpecialCharRuleValidation = 4,
        MustContainUppercaseRuleValidation = 5,
        NoRepeatedCharsRuleValidation = 6,
        WhitespaceNotAllowed = 7,
        NullNotAllowed = 8
    }
}

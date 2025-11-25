namespace ValidadorSenhaSegura.Domain.Enums
{
    public enum RulesetValidationErrorCode
    {
        NotSet = -1,
        MinLengthRuleValidation = 1,              
        NoRepeatedCharsRuleValidation = 2,
        WhitespaceNotAllowed = 3,
        MinimumRequirementsRuleValidation = 4,
    }
}

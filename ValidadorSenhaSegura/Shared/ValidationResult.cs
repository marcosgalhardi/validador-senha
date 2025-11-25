using ValidadorSenhaSegura.Domain.Enums;

namespace ValidadorSenhaSegura.Shared
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<Error> Errors { get; private set; } = [];

        public static ValidationResult Success() => new();
        
        public static ValidationResult Failure(IEnumerable<Error> errors) =>
            new ValidationResult
            {
                Errors = errors.ToList()
            };

        public static ValidationResult Failure(IEnumerable<string> errors) =>
            new ValidationResult
            {
                Errors = errors
                    .Select(error => Error.Create(error, RulesValidationErrorCode.NotSet))
                    .ToList()
            };

        public static ValidationResult Failure(string error) =>
            new ValidationResult
            {
                Errors = [ 
                    Error.Create(error, RulesValidationErrorCode.NotSet) 
                ]
            };      
        }
}

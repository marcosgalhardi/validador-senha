using ValidadorSenhaSegura.Shared.Extensions;

namespace ValidadorSenhaSegura.Shared
{
    public sealed class Error
    {
        public int Code { get; set; } = -1;
        public required string Message { get; set; }

        public Error() { }

        public Error(string message, int? code)
        {
            Code = code ?? -1;
            Message = message;
        }

        public static Error Create<TEnum>(string message, TEnum code) where TEnum : Enum
        {
            return Create(message, code.EnumToInt());
        }

        public static Error Create(string message, int code)
        {
            return new Error { Message = message, Code = code };
        }

        public static Error Create(string message)
        {
            return new Error { Message = message };
        }
    }
}

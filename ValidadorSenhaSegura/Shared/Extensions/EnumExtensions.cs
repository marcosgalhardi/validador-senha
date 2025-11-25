namespace ValidadorSenhaSegura.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static int EnumToInt<TEnum>(this TEnum value) where TEnum : Enum
        {
            return Convert.ToInt32(value);
        }
    }
}

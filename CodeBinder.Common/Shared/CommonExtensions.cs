using System.Diagnostics.CodeAnalysis;

namespace CodeBinder.Shared
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Lowercase the first character of the identifier
        /// </summary>
        public static string ToLowerCamelCase(this string text)
        {
            if (text.IsNullOrEmpty() || char.IsLower(text, 0))
                return text;

            return char.ToLowerInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// Uppercase the first character of the identifier
        /// </summary>
        public static string ToUpperCamelCase(this string text)
        {
            if (text.IsNullOrEmpty() || char.IsUpper(text, 0))
                return text;

            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        public static bool IsNullOrEmpty([NotNullWhen(false)]this string? value)
        {
            return value == null || value.Length == 0;
        }
    }
}

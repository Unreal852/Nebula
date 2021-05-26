using System.IO;

namespace Nebula.Utils.Extensions
{
    /// <summary>
    /// Provide extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Remove invalid chars from the specified string.
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="replaceWith">Replace char</param>
        /// <returns>Clean string</returns>
        public static string ReplaceInvalidPathChars(this string str, string replaceWith = "")
        {
            return string.Join(replaceWith, str.Split(Path.GetInvalidFileNameChars()));
        }

        /// <summary>
        /// Truncate a string if it is bigger than the specified max length.
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Truncated value</returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
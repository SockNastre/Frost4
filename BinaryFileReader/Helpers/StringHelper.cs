namespace BinaryFileReader.Helpers
{
    /// <summary>
    /// Provides helper methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Formats <see cref="string"/> object to display properly if length exceeds <paramref name="maxTextLength"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> object to format.</param>
        /// <param name="maxTextLength">Maximum length for inputted <see cref="string"/> value.</param>
        /// <param name="lastChar">The <see cref="char"/> object to append to formatted string.</param>
        /// <returns>Formatted value as <see cref="string"/> object.</returns>
        public static string FormatForLength(this string value, int maxTextLength, char lastChar = ' ')
        {
            // If value exceeds the maxTextLength then data is optimized with "..." at the end
            // maxTextLength is subtracted by 4 to make space for "..." then a character after that
            return value.Length > maxTextLength ? $"{value.Remove(maxTextLength - 4)}...{lastChar}" : value;
        }
    }
}

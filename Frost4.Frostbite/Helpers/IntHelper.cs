using System;

namespace Frost4.Frostbite.Helpers
{
    /// <summary>
    /// Helper methods for the <see cref="int"/> class.
    /// </summary>
    public static class IntHelper
    {
        /// <summary>
        /// Calculates length of variable integer bytes generated from an <see cref="int"/> object.
        /// </summary>
        /// <param name="value">The <see cref="int"/> to calculate variable bytes size from.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> parameter is negative number.</exception>
        /// <returns>Variable bytes length as <see cref="int"/> object.</returns>
        public static int CalculateVariableBytesLength(this int value)
        {
            // Negative numbers are not accepted
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Value must be non-negative number.");
            }

            // Final length of variable bytes that will be returned at the end
            int length = 0;

            bool isFirstRun = true;
            while (isFirstRun || value > 0)
            {
                isFirstRun = false;

                value >>= 7;
                length++;
            }

            return length;
        }
    }
}

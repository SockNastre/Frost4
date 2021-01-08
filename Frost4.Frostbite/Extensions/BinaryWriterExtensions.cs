using System;
using System.IO;

namespace Frost4.Frostbite.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BinaryWriter"/> class.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes 0x00, '\0' character: the null-terminator byte from <see cref="Constants.NullTerminator"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        public static void WriteNullTerminator(this BinaryWriter writer)
        {
            writer.Write(Constants.NullTerminator);
        }

        /// <summary>
        /// Writes null-terminated string using a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <param name="value">The <see cref="string"/> to write as null-terminated string.</param>
        /// <remarks>
        /// See <see cref="BinaryReaderExtensions.ReadNullTerminatedString(BinaryReader)"/> for how to read null-terminated string with <see cref="BinaryReader"/>
        /// </remarks>
        public static void WriteNullTerminatedString(this BinaryWriter writer, string value)
        {
            foreach (char c in value)
            {
                writer.Write(c);
            }

            writer.WriteNullTerminator();
        }

        /// <summary>
        /// Writes a variable-byte signed integer to the current stream and advances the stream position by those variable bytes.
        /// <see href="https://stackoverflow.com/a/3564685/10216412">SOURCE</see>
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <param name="value">The <see cref="int"/> to write as variable integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> parameter is negative number.</exception>
        /// <remarks>
        /// See <see cref="BinaryReaderExtensions.ReadVariableInt32(BinaryReader)"/> for how to decode the value with a <see cref="BinaryReader"/>.
        /// </remarks>
        public static void WriteVariableInt(this BinaryWriter writer, int value)
        {
            // Negative numbers are not accepted
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "Value must be non-negative number.");
            }

            bool isFirstRun = true;
            while (isFirstRun || value > 0)
            {
                isFirstRun = false;
                var lower7bits = (byte)(value & 0x7f);

                value >>= 7;
                if (value > 0)
                {
                    lower7bits |= 128;
                }

                writer.Write(lower7bits);
            }
        }
    }
}

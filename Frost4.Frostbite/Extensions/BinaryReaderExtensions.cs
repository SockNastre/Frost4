using System.IO;
using System.Text;

namespace Frost4.Frostbite.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BinaryReader"/> class.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads null-terminated string using a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
        /// <remarks>
        /// See <see cref="BinaryWriterExtensions.WriteNullTerminatedString(BinaryWriter, string)"/> for how to write null-terminated string with <see cref="BinaryWriter"/>
        /// </remarks>
        /// <returns>Null-terminated string as <see cref="string"/> object.</returns>
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var sb = new StringBuilder();
            byte charByte = reader.ReadByte();

            // 0x00 is a null-terminator byte, marking end of character array
            while (charByte != Constants.NullTerminator)
            {
                sb.Append((char)charByte);
                charByte = reader.ReadByte();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Decodes an <see cref="int"/> value from a variable number of bytes, known as a "variable integer".
        /// <see href="https://stackoverflow.com/a/3564685/10216412">SOURCE</see>
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
        /// <remarks>
        /// See <see cref="BinaryWriterExtensions.WriteVariableInt(BinaryWriter, int)"/> for how to encode the value with a <see cref="BinaryWriter"/>.
        /// </remarks>
        /// <returns>Variable integer bytes as <see cref="int"/> object.</returns>
        public static int ReadVariableInt32(this BinaryReader reader)
        {
            bool isMoreData = true;
            int value = 0;
            int bitShift = 0;

            while (isMoreData)
            {
                byte lower7bits = reader.ReadByte();

                isMoreData = (lower7bits & 128) != 0;
                value |= (lower7bits & sbyte.MaxValue) << bitShift;
                bitShift += 7;
            }

            return value;
        }
    }
}

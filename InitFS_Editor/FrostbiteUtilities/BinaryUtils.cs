using System;
using System.IO;
using System.Text;

namespace InitFS_Editor.FrostbiteUtilities
{
    public static class BinaryUtils
    {
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var sb = new StringBuilder();
            byte c = reader.ReadByte();

            while (c != 0)
            {
                sb.Append((char)c);
                c = reader.ReadByte();
            }

            return sb.ToString();
        }

        public static void WriteNullTerminatedString(this BinaryWriter writer, string value)
        {
            foreach (char c in value)
            {
                writer.Write(c);
            }

            writer.Write((byte)0); // Null terminator
        }

        /// <summary>
        /// Encodes the specified <see cref="System.Int32"/> value with a variable number of
        /// bytes, and writes the encoded bytes to the specified writer.
        /// https://stackoverflow.com/a/3564685/10216412
        /// </summary>
        /// <param name="writer">
        /// The <see cref="BinaryWriter"/> to write the encoded value to.
        /// </param>
        /// <param name="value">
        /// The <see cref="System.Int32"/> value to encode and write to the <paramref name="writer"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="writer"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="value"/> is less than 0.</para>
        /// </exception>
        /// <remarks>
        /// See <see cref="DecodeInt32"/> for how to decode the value back from
        /// a <see cref="BinaryReader"/>.
        /// </remarks>
        public static void WriteVarInt(this BinaryWriter writer, int value)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", value, "Value must be 0 or greater.");

            bool first = true;
            while (first || value > 0)
            {
                first = false;
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;
                if (value > 0)
                    lower7bits |= 128;
                writer.Write(lower7bits);
            }
        }

        /// <summary>
        /// Encodes the specified <see cref="System.Int32"/> value with a variable number of
        /// bytes, and returns the length of those bytes.
        /// https://stackoverflow.com/a/3564685/10216412
        /// </summary>
        public static int CalculateWrittenVarIntSize(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", value, "Value must be 0 or greater.");

            int size = 0;
            bool first = true;
            while (first || value > 0)
            {
                first = false;
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;
                if (value > 0)
                    lower7bits |= 128;
                size++;
            }

            if (size == 0)
                throw new Exception("Size must be 1 or greater.");

            return size;
        }

        /// <summary>
        /// Decodes a <see cref="System.Int32"/> value from a variable number of
        /// bytes, originally encoded with <see cref="EncodeInt32"/> from the specified reader.
        /// https://stackoverflow.com/a/3564685/10216412
        /// </summary>
        /// <param name="reader">
        /// The <see cref="BinaryReader"/> to read the encoded value from.
        /// </param>
        /// <returns>
        /// The decoded <see cref="System.Int32"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="reader"/> is <c>null</c>.</para>
        /// </exception>
        public static int ReadVarInt(this BinaryReader reader)
        {
            bool more = true;
            int value = 0;
            int shift = 0;
            while (more)
            {
                byte lower7bits = reader.ReadByte();
                more = (lower7bits & 128) != 0;
                value |= (lower7bits & 0x7f) << shift;
                shift += 7;
            }
            return value;
        }
    }
}
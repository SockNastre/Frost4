using System.IO;

namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Methods for reading/generating data from surface files.
    /// </summary>
    public static class SurfaceFile
    {
        /// <summary>
        /// Returns blank keys for header in file.
        /// </summary>
        public static byte[] GetBlankKeys => new byte[SurfaceConstants.KeysLength];

        /// <summary>
        /// Checks for header in data using a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with</param>
        /// <returns>If header exists or not.</returns>
        public static bool IsHeaderPresent(BinaryReader reader) => reader.ReadInt32() == SurfaceConstants.HeaderMagic;

        /// <summary>
        /// Checks for header in a file.
        /// </summary>
        /// <param name="path">The file to check header in.</param>
        /// <returns>If header exists or not.</returns>
        public static bool IsHeaderPresent(string path)
        {
            return IsHeaderPresent(new BinaryReader(File.OpenRead(path)));
        }

        /// <summary>
        /// Creates <see cref="Entry"/> from surface-level Frostbite 3 file starting from specified offset.
        /// </summary>
        /// <param name="path">The file to open for creating <see cref="Entry"/>.</param>
        /// <param name="offset">Position to begin reading in inside of file.</param>
        /// <returns>The <see cref="Entry"/> object created from a file.</returns>
        public static Entry CreateEntry(string path, uint offset = 0)
        {
            using var reader = new BinaryReader(File.OpenRead(path));
            reader.BaseStream.Position = offset;

            return new Entry(reader);
        }

        /// <summary>
        /// Creates <see cref="Entry"/> from surface-level Frostbite 3 file with option to skip header.
        /// </summary>
        /// <param name="path">The file to open for creating <see cref="Entry"/>.</param>
        /// <param name="isHeaderSkipped">Indicates whether to attempt to skip DICE's 556-<see cref="byte"/> header or not.</param>
        /// <returns>The <see cref="Entry"/> object created from a file.</returns>
        public static Entry CreateEntry(string path, bool isHeaderSkipped)
        {
            uint offset = isHeaderSkipped ? SurfaceConstants.HeaderSize : 0;
            return SurfaceFile.CreateEntry(path, offset);
        }

        /// <summary>
        /// Writes <see cref="Entry"/> object as file with an optional header.
        /// </summary>
        /// <param name="entry">The <see cref="Entry"/> object to write as file.</param>
        /// <param name="path">Save path for file.</param>
        /// <param name="isHeaderCreated">Whether to create header in file or not.</param>
        /// <param name="keys">The <see cref="byte"/>[] data to write as keys in the header.<para>If set to null keys are null bytes.</para></param>
        /// <remarks>
        /// Uses <see cref="WriteHeader(BinaryWriter, byte[])"/> to write header for file if indicated to.
        /// </remarks>
        public static void Write(this Entry entry, string path, bool isHeaderCreated, byte[] keys = null)
        {
            using var writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write));
            
            if (isHeaderCreated)
            {
                SurfaceFile.WriteHeader(writer, keys);
            }

            entry.Write(writer);
        }

        /// <summary>
        /// Writes header using <see cref="BinaryWriter"/> adding optional <paramref name="keys"/> as <see cref="byte"/>[] data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <param name="keys">The <see cref="byte"/>[] data to write as keys in the header.</param>
        public static void WriteHeader(BinaryWriter writer, byte[] keys = null)
        {
            writer.Write(SurfaceConstants.HeaderMagic);
            writer.Write(keys ?? SurfaceFile.GetBlankKeys);
        }
    }
}

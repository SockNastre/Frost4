namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Format of data stored inside <see cref="Entry"/> object.
    /// </summary>
    public enum EntryFormat : byte
    {
        /// <summary>
        /// Array of <see cref="Entry"/> format byte.
        /// </summary>
        EntryArray = 0x81,
        /// <summary>
        /// List of <see cref="Flag"/> format byte.
        /// </summary>
        FlagList = 0x82,
        /// <summary>
        /// <see cref="string"/> type format byte.
        /// </summary>
        String = 0x87,
        /// <summary>
        /// ID for install chunk which is 16 bytes long.
        /// </summary>
        ChunkId = 0x8F,
    }
}

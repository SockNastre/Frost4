namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Format of data stored inside <see cref="Flag"/> object.
    /// </summary>
    public enum FlagFormat : byte
    {
        /// <summary>
        /// Array of <see cref="Entry"/> format byte.
        /// </summary>
        EntryArray = 0x01,
        /// <summary>
        /// List of <see cref="Flag"/> format byte.
        /// </summary>
        FlagList = 0x02,
        /// <summary>
        /// <see cref="bool"/> type format byte.
        /// </summary>
        Boolean = 0x06,
        /// <summary>
        /// <see cref="string"/> type format byte.
        /// </summary>
        String = 0x07,
        /// <summary>
        /// <see cref="int"/> type format byte.
        /// </summary>
        Integer32 = 0x08,
        /// <summary>
        /// <see cref="long"/> type format byte.
        /// </summary>
        Integer64 = 0x09,
        /// <summary>
        /// <see cref="float"/> type format byte.
        /// </summary>
        Float = 0x0B,
        /// <summary>
        /// ID which is 16 bytes long.
        /// </summary>
        Id = 0x0F,
        /// <summary>
        /// SHA1 hash which is 20 bytes long.
        /// </summary>
        Sha1 = 0x10,
        /// <summary>
        /// <see cref="byte"/>[] type format byte.
        /// </summary>
        ByteArray = 0x13,
    }
}

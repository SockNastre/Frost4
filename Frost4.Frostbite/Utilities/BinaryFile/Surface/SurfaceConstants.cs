namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Constant fields used throughout surface-level binary file classes.
    /// </summary>
    public static class SurfaceConstants
    {
        /// <summary>
        /// Size of header given to binary files if they have one.
        /// </summary>
        public const int HeaderSize = 556;

        /// <summary>
        /// Magic used in headers.
        /// </summary>
        public const int HeaderMagic = 0x01CED100;

        /// <summary>
        /// Length of keys in header.
        /// </summary>
        public const int KeysLength = 552;
    }
}

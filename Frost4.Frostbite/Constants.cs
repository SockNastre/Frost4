namespace Frost4.Frostbite
{
    /// <summary>
    /// Constant fields used throughout other classes for Frostbite.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The null-terminator byte.
        /// </summary>
        public const byte NullTerminator = 0x00;

        /// <summary>
        /// Length of <see cref="NullTerminator"/> in memory.
        /// </summary>
        public const int NullTerminatorLength = sizeof(byte);
    }
}

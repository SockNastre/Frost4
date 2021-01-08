using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System;
using System.Windows.Forms;

namespace BinaryFileReader.Utilities.TreeNodeAdditions
{
    /// <summary>
    /// Stores/formats data from <see cref="Entry"/> and <see cref="Flag"/> objects for use in <see cref="TreeNode"/> tag.
    /// </summary>
    public class EntryFlagDataTag
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EntryFlagDataTag"/> class from <see cref="Entry"/> object.
        /// </summary>
        /// <param name="entry"><see cref="Entry"/> object to initialize <see cref="EntryFlagDataTag"/> with.</param>
        public EntryFlagDataTag(Entry entry)
        {
            this.IsFlag = false;
            this.Format = Enum.GetName(typeof(EntryFormat), entry.Format);
            this.Name = "NULL";

            switch (entry.Format)
            {
                case EntryFormat.String:
                    {
                        var data = (string)entry.Data;
                        this.Data = data;
                        break;
                    }

                case EntryFormat.ChunkId:
                    {
                        var data = (byte[])entry.Data;
                        string parsedData = BitConverter.ToString(data)
                            .Replace("-", "")
                            .ToLowerInvariant();
                        this.Data = parsedData;

                        break;
                    }

                default:
                    {
                        this.Data = null;
                        break;
                    }
            }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="EntryFlagDataTag"/> class from <see cref="Flag"/> object.
        /// </summary>
        /// <param name="flag"><see cref="Flag"/> object to initialize <see cref="EntryFlagDataTag"/> with.</param>
        public EntryFlagDataTag(Flag flag)
        {
            this.IsFlag = true;
            this.Format = Enum.GetName(typeof(FlagFormat), flag.Format);
            this.Name = $"\"{flag.Name}\"";

            switch (flag.Format)
            {
                case FlagFormat.EntryArray:
                case FlagFormat.FlagList:
                    {
                        this.Data = null;
                        break;
                    }

                case FlagFormat.String:
                    {
                        var data = (string)flag.Data;
                        this.Data = data;
                        break;
                    }

                case FlagFormat.Id:
                case FlagFormat.Sha1:
                case FlagFormat.ByteArray:
                    {
                        var data = (byte[])flag.Data;
                        string parsedData = BitConverter.ToString(data).Replace("-", "");

                        if (flag.Format != FlagFormat.ByteArray)
                        {
                            // Formats other than ByteArray are better represented all lowercase for the bytes as a string
                            parsedData = parsedData.ToLowerInvariant();
                        }

                        this.Data = parsedData;
                        break;
                    }

                default:
                    {
                        string data = flag.Data.ToString();
                        this.Data = data;
                        break;
                    }
            }
        }

        /// <summary>
        /// Name given to data, only used for <see cref="Flag"/> type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of data, representing <see cref="Entry"/> (False) or <see cref="Flag"/> (True).
        /// </summary>
        public bool IsFlag { get; set; }

        /// <summary>
        /// Format of data represented as <see cref="string"/> object from either <see cref="EntryFormat"/> or <see cref="FlagFormat"/>.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Data associated with <see cref="Entry"/>/<see cref="Flag"/> represented as <see cref="string"/> object.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gives data as formatted string based on certain properties in <see cref="EntryFlagDataTag"/> object.
        /// </summary>
        public string GiveMetadata => $"Name: {this.Name}\n" +
            $"Type: {(this.IsFlag ? "Flag" : "Entry")}\n" +
            $"Format: {this.Format}\n";
    }
}

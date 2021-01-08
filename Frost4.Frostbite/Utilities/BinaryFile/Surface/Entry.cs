using Frost4.Frostbite.Extensions;
using Frost4.Frostbite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Stores primary data of a Frostbite 3 binary file section.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        public Entry() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class with a <see cref="BinaryReader"/> to read data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
        public Entry(BinaryReader reader)
        {
            this.Read(reader);
        }

        /// <summary>
        /// Gets or sets the <see cref="EntryFormat"/> of the current <see cref="Entry"/> object.
        /// </summary>
        public EntryFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the data stored in the current <see cref="Entry"/> object.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Reads data using a <see cref="BinaryReader"/> to this instance.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
        /// <remarks>
        /// See <see cref="Write(BinaryWriter)"/> for how to write data with <see cref="BinaryWriter"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">If <see cref="Entry.Format"/> read is unknown.</exception>
        private void Read(BinaryReader reader)
        {
            this.Format = (EntryFormat)reader.ReadByte();

            switch (this.Format)
            {
                case EntryFormat.EntryArray:
                    _ = reader.ReadVariableInt32(); // Length of list
                    var entryList = new List<Entry>();

                    // EntryList is null-terminated
                    while (reader.ReadByte() != Constants.NullTerminator)
                    {
                        reader.BaseStream.Position -= 1;

                        var entry = new Entry(reader);
                        entryList.Add(entry);
                    }

                    this.Data = entryList;
                    break;

                case EntryFormat.FlagList:
                    _ = reader.ReadVariableInt32(); // Length of list
                    var flagList = new List<Flag>();

                    // FlagList is null-terminated
                    while (reader.ReadByte() != Constants.NullTerminator)
                    {
                        reader.BaseStream.Position -= 1;

                        var flag = new Flag(reader);
                        flagList.Add(flag);
                    }

                    this.Data = flagList;
                    break;

                case EntryFormat.String:
                    _ = reader.ReadVariableInt32(); // Length of string, may not be variable int
                    this.Data = reader.ReadNullTerminatedString();
                    break;

                case EntryFormat.ChunkId:
                    this.Data = reader.ReadBytes(16);
                    break;

                default:
                    throw new InvalidOperationException("Unknown entry format byte.\n" +
                        $"Byte Offset: 0x{reader.BaseStream.Position - sizeof(EntryFormat):X2}");
            }
        }

        /// <summary>
        /// Writes data with a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <remarks>
        /// See <see cref="Read(BinaryReader)"/> for how to read data with <see cref="BinaryReader"/>.
        /// </remarks>
        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)this.Format);
            writer.WriteVariableInt(this.CalculateEntryDataSize());

            switch (this.Format)
            {
                case EntryFormat.EntryArray:
                    {
                        var data = (List<Entry>)Data;

                        foreach (Entry entry in data)
                        {
                            entry.Write(writer);
                        }

                        writer.WriteNullTerminator();
                        break;
                    }

                case EntryFormat.FlagList:
                    {
                        var data = (List<Flag>)Data;

                        foreach (Flag flag in data)
                        {
                            flag.Write(writer);
                        }

                        writer.WriteNullTerminator();
                        break;
                    }

                case EntryFormat.String:
                    {
                        var data = (string)Data;
                        writer.WriteVariableInt(data.Length);
                        writer.WriteNullTerminatedString(data);

                        break;
                    }

                case EntryFormat.ChunkId:
                    writer.Write((byte[])Data);
                    break;
            }
        }

        /// <summary>
        /// Calculates size of all data inside of <see cref="Entry"/>, not including <see cref="EntryFormat"/> and data size variable integer bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <see cref="Format"/> property is not set to value in <see cref="EntryFormat"/>.</exception>
        /// <returns>Entry data size as <see cref="int"/> object.</returns>
        public int CalculateEntryDataSize()
        {
            switch (this.Format)
            {
                case EntryFormat.EntryArray:
                    {
                        var data = (List<Entry>)Data;
                        int byteCount = 0;

                        foreach (Entry entry in data)
                        {
                            int entrySize = entry.CalculateEntryDataSize();
                            int entrySizeLength = entrySize.CalculateVariableBytesLength();

                            byteCount += entrySizeLength + entrySize + Constants.NullTerminatorLength;
                        }

                        return byteCount + Constants.NullTerminatorLength;
                    }

                case EntryFormat.FlagList:
                    {
                        var data = (List<Flag>)Data;
                        int byteCount = 0;

                        foreach (Flag flag in data)
                        {
                            int flagDataSize = flag.CalculateFlagDataSize();
                            byteCount += sizeof(FlagFormat) + flag.Name.Length + Constants.NullTerminatorLength + flagDataSize;

                            switch (flag.Format)
                            {
                                // These types have a variable int before data indicating the length of the data
                                case FlagFormat.EntryArray:
                                case FlagFormat.FlagList:
                                case FlagFormat.String:
                                case FlagFormat.ByteArray:
                                    {
                                        byteCount += flagDataSize.CalculateVariableBytesLength();
                                        break;
                                    }
                            }
                        }

                        return byteCount + Constants.NullTerminatorLength;
                    }

                case EntryFormat.String:
                    {
                        var data = (string)Data;
                        return data.Length + Constants.NullTerminatorLength;
                    }

                case EntryFormat.ChunkId:
                    return 16;

                default:
                    // Should never come here, but is required
                    throw new InvalidOperationException("Invalid entry format.");
            }
        }
    }
}

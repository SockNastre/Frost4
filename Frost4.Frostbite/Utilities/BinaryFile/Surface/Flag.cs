using Frost4.Frostbite.Extensions;
using Frost4.Frostbite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Frost4.Frostbite.Utilities.BinaryFile.Surface
{
    /// <summary>
    /// Stores secondary data of a Frostbite 3 binary file section.
    /// </summary>
    public class Flag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class.
        /// </summary>
        public Flag() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flag"/> class with a <see cref="BinaryReader"/> to read data.
        /// </summary>
        /// <param name="reader"><see cref="BinaryReader"/> to be used.</param>
        /// <exception cref="InvalidOperationException">If <see cref="Flag.Format"/> read is unknown.</exception>
        /// <remarks>
        /// See <see cref="Write(BinaryWriter)"/> for how to write data with <see cref="BinaryWriter"/>.
        /// </remarks>
        public Flag(BinaryReader reader)
        {
            this.Format = (FlagFormat)reader.ReadByte();
            this.Name = reader.ReadNullTerminatedString();

            switch (Format)
            {
                case FlagFormat.EntryArray:
                    {
                        _ = reader.ReadVariableInt32(); // Length of list
                        var data = new List<Entry>();

                        // EntryList is null-terminated
                        while (reader.ReadByte() != Constants.NullTerminator)
                        {
                            reader.BaseStream.Position -= 1;

                            var entry = new Entry(reader);
                            data.Add(entry);
                        }

                        this.Data = data;
                        break;
                    }

                case FlagFormat.FlagList:
                    {
                        _ = reader.ReadVariableInt32(); // Length of list
                        var data = new List<Flag>();

                        // FlagList is null-terminated
                        while (reader.ReadByte() != Constants.NullTerminator)
                        {
                            reader.BaseStream.Position -= 1;

                            var flag = new Flag(reader);
                            data.Add(flag);
                        }

                        this.Data = data;
                        break;
                    }

                case FlagFormat.Boolean:
                    this.Data = reader.ReadBoolean();
                    break;

                case FlagFormat.String:
                    _ = reader.ReadVariableInt32(); // Length of string, may not be variable int
                    this.Data = reader.ReadNullTerminatedString();
                    break;

                case FlagFormat.Integer32:
                    this.Data = reader.ReadInt32();
                    break;

                case FlagFormat.Integer64:
                    this.Data = reader.ReadInt64();
                    break;

                case FlagFormat.Float:
                    this.Data = reader.ReadSingle();
                    break;

                case FlagFormat.Id:
                    this.Data = reader.ReadBytes(16);
                    break;

                case FlagFormat.Sha1:
                    this.Data = reader.ReadBytes(20);
                    break;

                case FlagFormat.ByteArray:
                    int dataLength = reader.ReadVariableInt32();
                    this.Data = reader.ReadBytes(dataLength);
                    break;

                default:
                    throw new InvalidOperationException("Unknown flag format byte.\n" +
                        $"Byte Offset: 0x{reader.BaseStream.Position - (sizeof(FlagFormat) + this.Name.Length + Constants.NullTerminatorLength):X2}");
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="FlagFormat"/> of the current <see cref="Flag"/> object.
        /// </summary>
        public FlagFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Flag"/> object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data stored in the current <see cref="Flag"/> object.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Writes data with a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <remarks>
        /// No read methods exists for <see cref="Flag"/> objects, only the <see cref="Flag(BinaryReader)"/> constructor reads data.
        /// </remarks>
        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)this.Format);
            writer.WriteNullTerminatedString(Name);

            switch (this.Format)
            {
                case FlagFormat.EntryArray:
                    {
                        writer.WriteVariableInt(this.CalculateFlagDataSize());
                        var data = (List<Entry>)Data;

                        foreach (Entry entry in data)
                        {
                            entry.Write(writer);
                        }

                        writer.WriteNullTerminator();
                        break;
                    }

                case FlagFormat.FlagList:
                    {
                        writer.WriteVariableInt(this.CalculateFlagDataSize());
                        var data = (List<Flag>)Data;

                        foreach (Flag flag in data)
                        {
                            flag.Write(writer);
                        }

                        writer.WriteNullTerminator();
                        break;
                    }

                case FlagFormat.Boolean:
                    writer.Write((bool)Data);
                    break;

                case FlagFormat.String:
                    {
                        var data = (string)Data;
                        writer.WriteVariableInt(data.Length + Constants.NullTerminatorLength);
                        writer.WriteNullTerminatedString(data);

                        break;
                    }

                case FlagFormat.Integer32:
                    writer.Write((int)Data);
                    break;

                case FlagFormat.Integer64:
                    writer.Write((long)Data);
                    break;

                case FlagFormat.Float:
                    writer.Write((float)Data);
                    break;

                case FlagFormat.Id:
                case FlagFormat.Sha1:
                    writer.Write((byte[])Data);
                    break;

                case FlagFormat.ByteArray:
                    {
                        var data = (byte[])Data;
                        writer.WriteVariableInt(data.Length);
                        writer.Write(data);

                        break;
                    }
            }
        }

        /// <summary>
        /// Calculates size of all data inside of <see cref="Flag"/>, not including <see cref="FlagFormat"/> byte and data size variable integer bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <see cref="Format"/> property is not set to value in <see cref="FlagFormat"/>.</exception>
        /// <returns>Flag data size as <see cref="int"/> object.</returns>
        public int CalculateFlagDataSize()
        {
            switch (this.Format)
            {
                case FlagFormat.EntryArray:
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

                case FlagFormat.FlagList:
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

                case FlagFormat.Boolean:
                    return sizeof(bool);

                case FlagFormat.String:
                    {
                        var data = (string)Data;
                        return data.Length + Constants.NullTerminatorLength;
                    }

                case FlagFormat.Integer32:
                    return sizeof(int);

                case FlagFormat.Float:
                    return sizeof(float);

                case FlagFormat.Integer64:
                    return sizeof(long);

                case FlagFormat.Id:
                    return 16;

                case FlagFormat.Sha1:
                    return 20;

                case FlagFormat.ByteArray:
                    {
                        var data = (byte[])Data;
                        return data.Length;
                    }

                default:
                    // Should never come here, but is required
                    throw new InvalidOperationException("Invalid flag format.");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InitFS_Editor.FrostbiteUtilities
{
    /*
     * Source: Frost4_MEC_Core\FrostbiteBinaryFile.cs
     * By: SockNastre
     * 
     * Is a modified version of the Frost4_MEC_Core Frostbite Binary File reading class that is
     * watered down for supporting reading initfs files. Is only made to support MEC so it isn't
     * guaranteed to work on other games especially if encryption is used.
     */

    public class Entry
    {
        public EntryType Type { get; set; }
        public object Data { get; set; }

        public Entry() { }
        public Entry(BinaryReader reader)
        {
            Type = (EntryType)reader.ReadByte();

            switch (Type)
            {
                default:
                    {
                        throw new ArgumentException("Invalid entry type.\n\nOffset: 0x" + (reader.BaseStream.Position - 1).ToString("X2"), "Type");
                    }

                case EntryType.EntryList:
                    {
                        _ = reader.ReadVarInt(); // Length of list
                        var entryList = new List<Entry>();

                        while (reader.ReadByte() != 0)
                        {
                            reader.BaseStream.Position -= 1;

                            var entry = new Entry(reader);
                            entryList.Add(entry);
                        }

                        Data = entryList;
                        break;
                    }

                case EntryType.FlagList:
                    {
                        _ = reader.ReadVarInt(); // Length of list
                        var flagList = new List<Flag>();

                        while (reader.ReadByte() != 0)
                        {
                            reader.BaseStream.Position -= 1;

                            var flag = new Flag(reader);
                            flagList.Add(flag);
                        }

                        Data = flagList;
                        break;
                    }
            }
        }

        public void WriteToFile(string path, long offset = 0, bool writeDiceHeader = false, byte[] diceKeys = null)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", offset, "Offset must be greater than or equal to 0.");

            using (var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
            {
                if (offset != 0)
                {
                    long dataLength = new FileInfo(path).Length;

                    // If the file already exists, and the offset is bigger than the length of the data
                    // then space between offset and the data end position is padded
                    if (dataLength < offset)
                    {
                        writer.BaseStream.Position = dataLength; // Goes to end of file
                        writer.Write(new byte[offset - dataLength]);
                    }
                    
                    writer.BaseStream.Position = offset;
                }

                if (writeDiceHeader)
                {
                    // Dice magic
                    writer.Write(0x01CED100);

                    if (diceKeys != null)
                    {
                        if (diceKeys.Count() == 552)
                        {
                            writer.Write(diceKeys);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException("diceKeys", diceKeys, "Key bytes out of acceptable range, there must be 552 bytes.");
                        }
                    }
                    else
                    {
                        // If no keys are inputted then key section is zero'd
                        writer.Write(new byte[552]);
                    }
                }

                this.Write(writer);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.WriteVarInt(this.CalculateEntryDataSize());

            switch (Type)
            {
                case EntryType.EntryList:
                    {
                        var data = (List<Entry>)Data;

                        foreach (Entry entry in data)
                        {
                            entry.Write(writer);
                        }

                        writer.Write((byte)0); // Null terminator;
                        break;
                    }

                case EntryType.FlagList:
                    {
                        var data = (List<Flag>)Data;

                        foreach (Flag flag in data)
                        {
                            flag.Write(writer);
                        }

                        writer.Write((byte)0); // Null terminator;
                        break;
                    }
            }
        }

        public int CalculateEntryDataSize()
        {
            switch (Type)
            {
                default:
                    {
                        throw new ArgumentException("Invalid entry type.", "Type");
                    }

                case EntryType.EntryList:
                    {
                        var data = (List<Entry>)Data;
                        int byteCount = 0;

                        foreach (Entry entry in data)
                        {
                            int entrySize = entry.CalculateEntryDataSize();
                            byteCount += 1 + BinaryUtils.CalculateWrittenVarIntSize(entrySize) + entrySize;
                        }

                        return byteCount + 1; // Plus null terminator
                    }

                case EntryType.FlagList:
                    {
                        var data = (List<Flag>)Data;
                        int byteCount = 0;

                        foreach (Flag flag in data)
                        {
                            int flagDataSize = flag.CalculateFlagDataSize();

                            // FlagType byte + flag name length + null terminator byte + flag data size
                            byteCount += 1 + flag.Name.Length + 1 + flagDataSize;

                            switch (flag.Type)
                            {
                                // These types have a variable int before data indicating the length of the data
                                case FlagType.FlagList:
                                case FlagType.Str:
                                case FlagType.VariableBytes:
                                    {
                                        byteCount += BinaryUtils.CalculateWrittenVarIntSize(flagDataSize);
                                        break;
                                    }
                            }
                        }

                        return byteCount + 1; // Plus null terminator
                    }
            }
        }
    }

    public class Flag
    {
        public FlagType Type { get; set; }
        public string Name { get; set; }
        public object Data { get; set; }

        public Flag() { }
        public Flag(BinaryReader reader)
        {
            Type = (FlagType)reader.ReadByte();
            Name = reader.ReadNullTerminatedString();

            switch (Type)
            {
                default:
                    {
                        throw new ArgumentException("Invalid flag type.\n\nOffset: 0x" + (reader.BaseStream.Position - (Name.Length + 2)).ToString("X2"), "Type");
                    }

                case FlagType.FlagList:
                    {
                        _ = reader.ReadVarInt(); // Length of list
                        var data = new List<Flag>();

                        while (reader.ReadByte() != 0)
                        {
                            reader.BaseStream.Position -= 1;

                            var flag = new Flag(reader);
                            data.Add(flag);
                        }

                        Data = data;
                        break;
                    }

                case FlagType.Str:
                    {
                        _ = reader.ReadVarInt(); // Length of string, idk if this is a variable int or not
                        Data = reader.ReadNullTerminatedString();

                        break;
                    }

                case FlagType.VariableBytes:
                    {
                        int dataLength = reader.ReadVarInt();
                        Data = reader.ReadBytes(dataLength);

                        break;
                    }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.WriteNullTerminatedString(Name);

            switch (Type)
            {
                case FlagType.FlagList:
                    {
                        writer.WriteVarInt(this.CalculateFlagDataSize());
                        var data = (List<Flag>)Data;

                        foreach (Flag flag in data)
                        {
                            flag.Write(writer);
                        }

                        writer.Write((byte)0); // Null terminator;
                        break;
                    }

                case FlagType.Str:
                    {
                        var data = (string)Data;
                        writer.WriteVarInt(data.Length + 1); // Plus null terminator
                        writer.WriteNullTerminatedString(data);

                        break;
                    }

                case FlagType.VariableBytes:
                    {
                        var data = (byte[])Data;
                        writer.WriteVarInt(data.Length);
                        writer.Write(data);

                        break;
                    }
            }
        }

        public int CalculateFlagDataSize()
        {
            switch (Type)
            {
                default:
                    {
                        throw new ArgumentException("Invalid flag type.", "Type");
                    }

                case FlagType.FlagList:
                    {
                        var data = (List<Flag>)Data;
                        int byteCount = 0;

                        foreach (Flag flag in data)
                        {
                            int flagDataSize = flag.CalculateFlagDataSize();

                            // FlagType byte + flag name length + null terminator byte + flag data size
                            byteCount += 1 + flag.Name.Length + 1 + flagDataSize;

                            switch (flag.Type)
                            {
                                // These types have a variable int before data indicating the length of the data
                                case FlagType.FlagList:
                                case FlagType.Str:
                                case FlagType.VariableBytes:
                                    {
                                        byteCount += BinaryUtils.CalculateWrittenVarIntSize(flagDataSize);
                                        break;
                                    }
                            }
                        }

                        return byteCount + 1; // Plus null terminator
                    }

                case FlagType.Str:
                    {
                        var data = (string)Data;
                        return data.Length + 1; // The value at the end is the null terminator byte
                    }

                case FlagType.VariableBytes:
                    {
                        var data = (byte[])Data;
                        return data.Length;
                    }
            }
        }
    }

    public enum EntryType : byte
    {
        EntryList = 0x81,
        FlagList = 0x82
    }

    public enum FlagType : byte
    {
        FlagList = 0x02,
        Str = 0x07,
        VariableBytes = 0x13
    }
}
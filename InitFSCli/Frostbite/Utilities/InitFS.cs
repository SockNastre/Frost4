using Frost4.Frostbite.Utilities.BinaryFile.Surface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InitFSCli.Frostbite.Utilities
{
    /// <summary>
    /// Manages initfs file as an object for this tool. Stores data and provides methods to deal with initfs.
    /// </summary>
    public class InitFS
    {
        /// <summary>
        /// Heading added in metadata text file.
        /// </summary>
        private readonly string MetaHeading = "# Created from Frost4's InitFSCli, is used later for repacking.\n" + 
            "# DO NOT DELETE IF YOU WANT TO REPACK";

        /// <summary>
        /// File name that metadata text file is saved with.
        /// </summary>
        private readonly string MetaFileName = "META.TXT";

        /// <summary>
        /// Initializes <see cref="InitFS"/> object from a file or directory.
        /// </summary>
        /// <param name="input">The file/directory to open and create <see cref="InitFS"/> object from.</param>
        /// <exception cref="ArgumentException">Thrown to indicate <paramref name="input"/> isn't a valid file path or directory.</exception>
        public InitFS(string input)
        {
            if (File.Exists(input))
            {
                this.ReadFromFile(input);
            }
            else if (Directory.Exists(input))
            {
                this.ReadFromDirectory(input);
            }
            else
            {
                // If parameter "input" is neither an existing file or directory.
                throw new ArgumentException("Data does not point to existing file or directory on system.", "input");
            }
        }

        /// <summary>
        /// Name of file that <see cref="InitFS"/> object was initialized from.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indication of if <see cref="InitFS"/> object is Windows (Win32) variation or something else.
        /// </summary>
        public bool IsWin32 { get; set; }

        /// <summary>
        /// Keys for Frostbite 3 engine that are used by <see cref="InitFS"/> object.
        /// </summary>
        /// <remarks>
        /// Is 552 bytes long. When saved to metadata text file is saved using <see cref="Convert.ToBase64String(byte[])"/>.
        /// </remarks>
        public byte[] Keys { get; set; }

        /// <summary>
        /// List of files in <see cref="InitFS"/> object as a list of <see cref="Entry"/>
        /// </summary>
        public List<Entry> FileEntryList { get; set; }

        /// <summary>
        /// Extracts all data as files with a metadata text file.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="ThrowNullReferenceException(string)"/> when required flag doesn't exist
        /// </remarks>
        /// <param name="directory"></param>
        public void ExtractAllData(string directory)
        {
            // Ensures that there is a backwards slash at the end of the directory string
            directory += '\\';

            Directory.CreateDirectory(directory);
            string metadata = $"{this.MetaHeading}\n\n" +
                $"{this.Name}\n" +
                $"{this.IsWin32}\n" + 
                Convert.ToBase64String(this.Keys);

            foreach (Entry fileEntry in this.FileEntryList)
            {
                var flagList = (List<Flag>)fileEntry.Data;

                foreach(Flag flag in flagList)
                {
                    if (flag.Name.Equals("$file"))
                    {
                        var subFlagList = (List<Flag>)flag.Data;

                        // Flags expected inside of "$file" flag
                        string fs = string.Empty;
                        string name = null;
                        byte[] payload = null;

                        foreach(Flag subFlag in subFlagList)
                        {
                            switch (subFlag.Name)
                            {
                                case "fs":
                                    fs = (string)subFlag.Data;
                                    break;

                                case "name":
                                    name = (string)subFlag.Data;
                                    break;

                                case "payload":
                                    payload = (byte[])subFlag.Data;
                                    break;
                            }
                        }

                        // Name flag should not be null
                        if (string.IsNullOrEmpty(name))
                        {
                            this.ThrowNullReferenceException("name");
                        }

                        // Payload should not be null
                        if (payload == null)
                        {
                            this.ThrowNullReferenceException("payload");
                        }

                        // Adds name and fs flag (if it isn't empty) to metadata as they are used for repacking
                        metadata += $"\n\n{name}";
                        if (!string.IsNullOrEmpty(fs))
                        {
                            metadata += $"\n{fs}";
                        }

                        // Getting directory of name for later usage in creating directory, not every name has a directory
                        string nameDirectory = Path.GetDirectoryName(name);

                        Directory.CreateDirectory(directory + nameDirectory);
                        File.WriteAllBytes(directory + name, payload);
                    }
                }
            }

            // Writes metadata as text file in root of directory
            File.WriteAllText($"{directory}\\{this.MetaFileName}", metadata);
        }

        /// <summary>
        /// Packs data from <see cref="FileEntryList"/> into a file.
        /// </summary>
        /// <param name="path">The file path to pack to.</param>
        public void PackData(string path)
        {
            var primaryEntry = new Entry
            {
                Format = EntryFormat.EntryArray,
                Data = this.FileEntryList,
            };

            primaryEntry.Write(path, this.IsWin32, this.Keys);
        }

        /// <summary>
        /// Reads data from file to put into current <see cref="InitFS"/> object.
        /// </summary>
        /// <param name="path">The file to read from.</param>
        /// <exception cref="ArgumentException">Thrown if file does not exist in <paramref name="path"/>.</exception>
        private void ReadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new ArgumentException("Data does not point to valid file on system.", "path");
            }

            using var reader = new BinaryReader(File.OpenRead(path));

            this.Name = Path.GetFileName(path);
            this.IsWin32 = SurfaceFile.IsHeaderPresent(reader);
            this.Keys = this.IsWin32 ? reader.ReadBytes(SurfaceConstants.KeysLength) : SurfaceFile.GetBlankKeys;
            this.FileEntryList = (List<Entry>)SurfaceFile.CreateEntry(path, this.IsWin32).Data;
        }

        /// <summary>
        /// Reads data from directory to put into current <see cref="InitFS"/> object.
        /// </summary>
        /// <param name="directory">The directory to read from.</param>
        /// <exception cref="ArgumentException">Thrown if metadata text file does not exist in <paramref name="directory"/>.</exception>
        private void ReadFromDirectory(string directory)
        {
            // Path of metadata text file in directory
            string metaPath = $"{directory}\\{this.MetaFileName}";

            if (!File.Exists(metaPath))
            {
                throw new ArgumentException("Data does not point to valid initfs directory on system.", "directory");
            }

            // First three items is the metadata header text and blank line, unnecessary
            string[] metadataText = File.ReadAllLines(metaPath).Skip(3).ToArray();

            // Reads first three lines from metadataText, which is then discarded plus a blank line after
            string initFSName = metadataText[0];
            bool isWin32 = bool.Parse(metadataText[1]);
            byte[] keys = isWin32 ? Convert.FromBase64String(metadataText[2]) : SurfaceFile.GetBlankKeys;
            metadataText = metadataText.Skip(4).ToArray();

            // Entry list to set to object
            var fileEntryList = new List<Entry>();

            for (uint i = 0; i < metadataText.Length; i++)
            {
                string filePath = metadataText[i];

                i++;
                string fs = i < metadataText.Length ? metadataText[i] : null;
                i += string.IsNullOrEmpty(fs) ? 0 : 1;

                var fsFlag = new Flag
                {
                    Format = FlagFormat.String,
                    Name = "fs",
                    Data = fs,
                };

                // Contains file path of file inside initfs (including file name)
                var nameFlag = new Flag
                {
                    Format = FlagFormat.String,
                    Name = "name",
                    Data = filePath
                };

                // Contains byte data of file
                var payloadFlag = new Flag
                {
                    Format = FlagFormat.ByteArray,
                    Name = "payload",
                    Data = File.ReadAllBytes($"{directory}\\{filePath}")
                };

                // Creates flag list and adds three (or two) previously initalized flag objects
                var subFlagList = new List<Flag>();

                // If fs is null it doesn't exist as part of the file's data
                if (!string.IsNullOrEmpty(fs))
                {
                    subFlagList.Add(fsFlag);
                }

                // Adds two other required flags that are part of file's data
                subFlagList.Add(nameFlag);
                subFlagList.Add(payloadFlag);

                var flagList = new List<Flag>()
                {
                    new Flag()
                        {
                            Format = FlagFormat.FlagList,
                            Name = "$file",
                            Data = subFlagList,
                        },
                };

                var fileEntry = new Entry
                {
                    Format = EntryFormat.FlagList,
                    Data = flagList,
                };

                fileEntryList.Add(fileEntry);
            }

            this.Name = initFSName;
            this.IsWin32 = isWin32;
            this.Keys = keys;
            this.FileEntryList = fileEntryList;
        }

        /// <summary>
        /// Throws <see cref="NullReferenceException"/> with given flag name as <see cref="string"/> object to indicate it is null.
        /// </summary>
        /// <param name="flag">The name of the <see cref="Flag"/> object to give error for.</param>
        /// <exception cref="NullReferenceException">Always thrown to indicate <paramref name="flag"/> value is associated with null data in <see cref="Flag"/> object.</exception>
        private void ThrowNullReferenceException(string flag)
        {
            throw new NullReferenceException($"Flag \"{flag}\" not found or contained null value, not expected.");
        }
    }
}

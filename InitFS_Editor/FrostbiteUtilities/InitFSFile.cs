using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace InitFS_Editor.FrostbiteUtilities
{
    [ReadOnly(true)]
    public class InitFSFile
    {
        [Category("$file")] public string FileSystem { get; set; }
        [Category("$file")] public string Name { get; set; }

        [Category("InitFS Editor")] public bool IsPayloadModified { get; set; } = false;
        [Browsable(false)] public byte[] OrgPayload { get; set; }
        [Browsable(false)] public byte[] ModifiedPayload { get; set; }

        public InitFSFile(List<Flag> flagList)
        {
            // Gets added to index later if FileSystem flag is present, faster than conditional operator I think
            int addVar = 0;

            if (flagList.Count == 3)
            {
                addVar = 1;
                FileSystem = (string)flagList[0].Data;
            }
            else if (flagList.Count != 2)
            {
                throw new ArgumentOutOfRangeException("Invalid number of flags in list, number must be 2 or 3.");
            }

            Name = (string)flagList[0 + addVar].Data;
            OrgPayload = (byte[])flagList[1 + addVar].Data;
        }

        public void ExportPayload(string path)
        {
            File.WriteAllBytes(path, IsPayloadModified ? ModifiedPayload : OrgPayload);
        }

        public void ResetPayload()
        {
            IsPayloadModified = false;
            ModifiedPayload = null;
        }
    }
}
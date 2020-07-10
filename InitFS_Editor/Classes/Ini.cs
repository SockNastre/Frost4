using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InitFS_Editor.Classes
{
    /*
     * Source: IniLib\Ini.cs
     * By: SockNastre
     * 
     * A prerelease Ini reading/writing class taken from IniLib for the purpose
     * of this tool.
     */

    public class Ini
    {
        public Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();

        public Ini() { }
        public Ini(string path)
        {
            this.ReadFromFile(path);
        }

        public void ReadFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            for (int i = 0; i < lines.Count(); i++)
            {
                string line = lines[i];

                // Blank lines are ignored
                if (!string.IsNullOrEmpty(line))
                {
                    // Lines starting with ';'(comments) are ignored and '[' is an identifier for a section
                    if (!line[0].Equals(';') && line[0].Equals('['))
                    {
                        Dictionary<string, string> sectionData = ReadSectionData(lines.Skip(i + 1).ToArray());
                        Data.Add(new string(line.Skip(1).Take(line.Count() - 2).ToArray()), sectionData);
                    }
                }
            }
        }

        public void WriteToFile(string path)
        {
            var iniSb = new StringBuilder();

            foreach (KeyValuePair<string, Dictionary<string, string>> sectionPropertyDictPair in Data)
            {
                iniSb.AppendLine('[' + sectionPropertyDictPair.Key + ']');

                // Reading keys
                foreach (KeyValuePair<string, string> propertyPair in sectionPropertyDictPair.Value)
                {
                    iniSb.AppendLine(propertyPair.Key + '=' + propertyPair.Value);
                }

                // Adds blank line to end of section
                iniSb.AppendLine();
            }

            File.WriteAllText(path, iniSb.ToString());
        }

        private Dictionary<string, string> ReadSectionData(string[] remainingLines)
        {
            var propertyDict = new Dictionary<string, string>();

            for (int i = 0; i < remainingLines.Count(); i++)
            {
                string line = remainingLines[i];

                // Blank lines are ignored
                if (!string.IsNullOrEmpty(line))
                {
                    // '[' is an identifier for a section
                    if (line[0].Equals('['))
                        break;

                    // Lines starting with ';'(comments) are ignored
                    if (!line[0].Equals(';'))
                    {
                        this.AddPropertyPair(propertyDict, line);
                    }
                }
            }

            return propertyDict;
        }

        private void AddPropertyPair(Dictionary<string, string> propertyDict, string line)
        {
            var keySb = new StringBuilder();
            var valSb = new StringBuilder();

            bool isKeyRead = false; // Indicates to start reading value

            for (int i = 0; i < line.Count(); i++)
            {
                char c = line[i];

                if (c.Equals('='))
                {
                    isKeyRead = true;
                }
                else
                {
                    if (isKeyRead)
                    {
                        // Goes onto reading value
                        valSb.Append(c);
                    }
                    else
                    {
                        // Keeps reading key
                        keySb.Append(c);
                    }
                }
            }

            propertyDict.Add(keySb.ToString(), valSb.ToString());
        }
    }
}
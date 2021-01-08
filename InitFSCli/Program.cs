using InitFSCli.Frostbite.Utilities;
using System;
using System.IO;
using System.Linq;

namespace InitFSCli
{
    /// <summary>
    /// The main entry class for the application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// <para>Checks arguments and runs program based on them.</para>
        /// </summary>
        /// <param name="args">Arguments passed through application.</param>
        private static void Main(string[] args)
        {
            // Program should not be ran with no arguments
            if (args.Length == 0)
            {
                Program.PrintHelp(true);
                return;
            }

            // One argument is assumed to be either a file or folder
            if (args.Length == 1)
            {
                if (File.Exists(args[0]))
                {
                    string initFSPath = args[0];
                    args = new string[5];

                    // Adds arguments to unpack initfs
                    args[0] = "-u";
                    args[1] = "-in";
                    args[2] = initFSPath;
                    args[3] = "-out";
                    args[4] = $"{initFSPath} out";
                }
                else if (Directory.Exists(args[0]))
                {
                    string directory = args[0];
                    args = new string[3];

                    // Adds arguments to unpack initfs
                    args[0] = "-p";
                    args[1] = "-in";
                    args[2] = directory;
                }
            }

            // Running program based on arguments
            switch (args[0])
            {
                case "-h":
                case "-help":
                    Program.PrintHelp();
                    break;

                case "-u":
                case "-unpack":
                    {
                        string inputPath = null;
                        string outputDirectory = null;

                        // Checks for input and output arguments
                        for (uint i = 1; i < args.Count(); i++)
                        {
                            string option = args[i].ToLower();

                            switch (option)
                            {
                                case "-in":
                                case "-input":
                                    i++;
                                    inputPath = args[i];
                                    break;

                                case "-out":
                                case "-output":
                                    i++;
                                    outputDirectory = args[i];
                                    break;

                                default:
                                    Program.PrintHelp(true);
                                    return;
                            }
                        }

                        // Input and outpute arguments are required
                        if (string.IsNullOrEmpty(inputPath))
                        {
                            Program.PrintHelp(true);
                            return;
                        }

                        // Extracts data
                        var initFS = new InitFS(inputPath);
                        initFS.ExtractAllData(outputDirectory);

                        break;
                    }

                case "-p":
                case "-pack":
                    {
                        string inputDirectory = null;
                        string outputPath = null;

                        // These options determine if a header is forcibly created or not
                        bool isHeaderForced = false;
                        bool isHeaderNotCreated = false;

                        // Can control keys in header
                        string externalKeysFile = null;

                        // Checks for input and output arguments
                        for (uint i = 1; i < args.Count(); i++)
                        {
                            string option = args[i].ToLower();

                            switch (option)
                            {
                                case "-in":
                                case "-input":
                                    i++;
                                    inputDirectory = args[i];
                                    break;

                                case "-out":
                                case "-output":
                                    i++;
                                    outputPath = args[i];
                                    break;

                                case "-fh":
                                case "-forceheader":
                                    isHeaderForced = true;
                                    break;

                                case "-keys":
                                    i++;
                                    externalKeysFile = args[i];
                                    break;

                                case "-fnh":
                                case "-forcenoheader":
                                    isHeaderNotCreated = true;
                                    break;

                                default:
                                    Program.PrintHelp(true);
                                    return;
                            }
                        }

                        // Invalid usage for bad options or incorrect pairings of options
                        if (string.IsNullOrEmpty(inputDirectory) 
                            || (isHeaderForced && isHeaderNotCreated)
                            || (!string.IsNullOrEmpty(externalKeysFile) && isHeaderNotCreated))
                        {
                            Program.PrintHelp(true);
                            return;
                        }

                        var initFS = new InitFS(inputDirectory);

                        // Sets up if header should be forced or forced not to be created
                        if (isHeaderForced)
                        {
                            initFS.IsWin32 = true;
                        }
                        else if (isHeaderNotCreated)
                        {
                            initFS.IsWin32 = false;
                        }

                        // Sets up optional external keys from another initfs
                        if (File.Exists(externalKeysFile))
                        {
                            var referenceInitFS = new InitFS(externalKeysFile);
                            initFS.Keys = referenceInitFS.Keys;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(externalKeysFile))
                            {
                                // If the user has setup the -keys option but the keys input file is invalid
                                Program.PrintHelp(true);
                                return;
                            }
                        }

                        if (string.IsNullOrEmpty(outputPath))
                        {
                            string inputDirectoryParent = Path.GetDirectoryName(inputDirectory);
                            outputPath = string.IsNullOrEmpty(inputDirectoryParent) ? initFS.Name : $"{inputDirectoryParent}\\{initFS.Name}";
                        }

                        initFS.PackData(outputPath);

                        break;
                    }

                default:
                    Program.PrintHelp(true);
                    break;
            }
        }

        /// <summary>
        /// Prints help text to console.
        /// </summary>
        /// <param name="isInvalidUsage">If method is ran from an invalid usage of arguments for the tool.</param>
        private static void PrintHelp(bool isInvalidUsage = false)
        {
            if (isInvalidUsage)
            {
                Console.WriteLine("Invalid usage\n");
            }

            Console.WriteLine("Frost4 | InitFSCli\n" +
                "Copyright (c) 2021 SockNastre\n" +
                "v1.0.0.0\n" +
                "\n" +
                "Usage: InitFSCli.exe <Command> <Options>\n" +
                "\n" +
                "Commands:\n" +
                "-help (-h)\n" +
                "-unpack (-u)\n" +
                "-pack (-p)\n" +
                "\n" +
                "(Un)Pack Options:\n" +
                "-input (-in) <PATH>\n" +
                "-output (-out) <PATH>\n" +
                "\n" +
                "Pack Options:\n" +
                "-forceheader (-fh)\n" +
                "-keys <PATH>\n" +
                "-forcenoheader (-fnh)\n" +
                "\n" +
                "Example Usages:\n" +
                "InitFSCli.exe -p -in \"initfs_Win32 Files\" -out \"initfs_Win32\"\n" +
                "              -u -in \"initfs_Win32\" -out \"initfs_Win32 Files\"\n" +
                "              -p -in \"initfs_Win32 Files\" -out \"initfs_Win32\" -keys \"keyfile_initfs\"");
        }
    }
}

using System;
using System.Windows.Forms;

namespace BinaryFileReader
{
    /// <summary>
    /// The main entry class for the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments passed through application.</param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Checks arguments to see if any file path might be passed through application
            // First argument is assumed to be a file path, only one argument can be passed through
            Application.Run(new MainForm(args.Length == 1 ? args[0] : null));
        }
    }
}

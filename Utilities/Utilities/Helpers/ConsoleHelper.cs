using System.Runtime.InteropServices;

namespace Utilities.Helpers
{
    public class ConsoleHelper
    {
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
    }
}
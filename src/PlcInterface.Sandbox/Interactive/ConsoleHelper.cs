using System;

namespace PlcInterface.Sandbox.Interactive
{
    /// <summary>
    /// Helpers for interacting with the <see cref="Console" />.
    /// </summary>
    internal static class ConsoleHelper
    {
        /// <summary>
        /// Write a colored line to the console and reset colors after.
        /// </summary>
        /// <param name="text">The <see cref="string" /> to write.</param>
        /// <param name="color">The <see cref="ConsoleColor" /> to write with.</param>
        public static void ConsoleWriteLineColored(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
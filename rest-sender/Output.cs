using System;

namespace rest_sender
{
    class Output
    {
        public const ConsoleColor Default = ConsoleColor.White;
        public const ConsoleColor Info = ConsoleColor.Cyan;
        public const ConsoleColor Error = ConsoleColor.Red;
        public const ConsoleColor Warning = ConsoleColor.Yellow;
        public const ConsoleColor Success = ConsoleColor.Green;

        public static string LogFile { get; set; } = string.Empty;

        public static void Write(string output)
        {
            Console.Write(output);
            WriteToFile(output);
        }

        public static void Write(ConsoleColor color, string output)
        {
            Console.ForegroundColor = color;
            Console.Write(output);
            Console.ResetColor();
            WriteToFile(output);
        }

        public static void Write(string format, params object[] values)
        {
            Console.Write(format, values);
            WriteToFile(format, values);
        }

        public static void Write(ConsoleColor color, string format, params object[] values)
        {
            Console.ForegroundColor = color;
            Console.Write(format, values);
            Console.ResetColor();
            WriteToFile(format, values);
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
            WriteLineToFile(output);
        }

        public static void WriteLine(ConsoleColor color, string output)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            Console.ResetColor();
            WriteLineToFile(output);
        }

        public static void WriteLine(string format, params object[] values)
        {
            Console.WriteLine(format, values);
            WriteLineToFile(format, values);
        }

        public static void WriteLine(ConsoleColor color, string format, params object[] values)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(format, values);
            Console.ResetColor();
            WriteLineToFile(format, values);
        }

        private static void WriteToFile(string output)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {

            }
        }

        private static void WriteToFile(string output, params object[] values)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {

            }
        }

        private static void WriteLineToFile(string output)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {

            }
        }

        private static void WriteLineToFile(string output, params object[] values)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {

            }
        }
    }
}

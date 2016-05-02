using System;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class ColoredConsoleTarget : Target
    {
        public ColoredConsoleTarget(IFormatter formatter = null)
            : base(formatter)
        {
        }

        public override void Write(Entry entry, IFormatter formatter)
        {
            lock (typeof (ColoredConsoleTarget))
            {
                var oldColor = Console.ForegroundColor;

                switch (entry.Level)
                {
                    case LogLevel.Warn:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;

                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;

                    case LogLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                }

                var content = (Formatter ?? formatter).Format(entry);

                Console.Write(content);

                Console.ForegroundColor = oldColor;
            }
        }

        public override void Write(string content)
        {
            throw new NotImplementedException();
        }
    }
}

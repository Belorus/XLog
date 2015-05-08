using System;

namespace XLog.ConsoleApp
{
    public class ConsoleTarget : Target
    {
        public ConsoleTarget(IFormatter formatter)
            : base(formatter)
        {
        }

        public override void Write(Entry entry)
        {
            Console.WriteLine(Formatter.Format(entry));
        }
    }
}

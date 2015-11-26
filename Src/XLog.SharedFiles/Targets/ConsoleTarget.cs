using System;

namespace XLog.NET.Targets
{
    public class ConsoleTarget : Target
    {
        public ConsoleTarget(IFormatter formatter = null)
            : base(formatter)
        {
        }

        public override void Write(string content)
        {
            Console.Write(content);
        }
    }
}

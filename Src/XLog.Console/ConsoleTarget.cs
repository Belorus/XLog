using System;

namespace XLog.ConsoleApp
{
    public class ConsoleTarget : Target
    {
        public ConsoleTarget(IFormatter formatter = null)
            : base(formatter)
        {
        }

        public override void Write(string content)
        {
            Console.WriteLine(content);
        }
    }
}

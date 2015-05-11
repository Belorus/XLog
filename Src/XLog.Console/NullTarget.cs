using System;

namespace XLog.ConsoleApp
{
    public class NullTarget : Target
    {
        public override void Write(string content)
        {
        }
    }
}

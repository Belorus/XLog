using System;

namespace XLog.Targets
{
    public class DebugTarget : Target
    {
        public DebugTarget(IFormatter formatter)
            : base(formatter)
        {
        }

        public override void Write(Entry entry)
        {
            System.Diagnostics.Debug.WriteLine(Formatter.Format(entry));
        }
    }
}

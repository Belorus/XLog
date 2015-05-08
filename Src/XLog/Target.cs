using System;

namespace XLog
{
    public abstract class Target
    {
        protected readonly IFormatter Formatter;

        protected Target(IFormatter formatter)
        {
            Formatter = formatter;
        }

        public abstract void Write(Entry entry);
    }
}

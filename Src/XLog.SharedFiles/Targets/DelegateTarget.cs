using System;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class DelegateTarget : Target
    {
        private readonly Action<string> _writerFunc;

        public DelegateTarget(Action<string> writerFunc, IFormatter formatter = null)
            : base(formatter)
        {
            _writerFunc = writerFunc;
        }

        public override void Write(string content)
        {
            _writerFunc(content);
        }
    }
}

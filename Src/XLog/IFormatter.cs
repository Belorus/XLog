using System;

namespace XLog
{
    public interface IFormatter
    {
        string Format(Entry entry);
    }

    public class NullFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            return string.Empty;
        }
    }
}

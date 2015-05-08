namespace XLog.Formatters
{
    public class NullFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            return string.Empty;
        }
    }
}

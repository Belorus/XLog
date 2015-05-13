
namespace XLog
{
    public abstract class Target
    {
        public readonly IFormatter Formatter;

        protected Target(IFormatter formatter = null)
        {
            Formatter = formatter;
        }

        public virtual void Write(Entry entry, IFormatter formatter)
        {
            var content = (Formatter ?? formatter).Format(entry);
            Write(content);
        }

        public abstract void Write(string content);

        public virtual void Flush()
        {
        }
    }
}

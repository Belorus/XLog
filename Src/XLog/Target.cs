namespace XLog
{
    public abstract class Target
    {
        public readonly IFormatter Formatter;

        protected Target(IFormatter formatter = null)
        {
            Formatter = formatter;
        }

        public abstract void Write(string content);
    }
}

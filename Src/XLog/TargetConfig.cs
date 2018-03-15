namespace XLog
{
    public class TargetConfig
    {
        public LogLevel MinLevel;
        public LogLevel MaxLevel;
        public readonly Target Target;

        public TargetConfig(LogLevel minLevel, LogLevel maxLevel, Target target)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Target = target;
        }

        public bool SupportsLevel(LogLevel level)
        {
            return MinLevel <= level && level <= MaxLevel;
        }
    }
}

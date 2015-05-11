namespace XLog
{
    internal class TargetConfig
    {
        internal readonly LogLevel MinLevel;
        internal readonly LogLevel MaxLevel;
        internal readonly Target Target;

        internal TargetConfig(LogLevel minLevel, LogLevel maxLevel, Target target)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Target = target;
        }

        internal bool SupportsLevel(LogLevel level)
        {
            return MinLevel <= level && level <= MaxLevel;
        }
    }
}

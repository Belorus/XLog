namespace XLog
{
    internal class TargetConfig
    {
        internal readonly int MinLevel;
        internal readonly int MaxLevel;
        internal readonly Target Target;

        internal TargetConfig(int minLevel, int maxLevel, Target target)
        {
            MinLevel = minLevel;
            MaxLevel = maxLevel;
            Target = target;
        }

        internal bool SupportsLevel(int level)
        {
            return MinLevel <= level && level <= MaxLevel;
        }
    }
}

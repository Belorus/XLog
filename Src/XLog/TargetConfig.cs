using System;

namespace XLog
{
    public class TargetConfig
    {
        public readonly int MinLevel;
        public readonly int MaxLevel;
        internal readonly Target Target;

        public TargetConfig(int minLevel, int maxLevel, Target target)
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

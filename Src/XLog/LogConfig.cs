namespace XLog
{
    public class LogConfig
    {
        internal TargetConfig[] TargetConfigs;
        internal readonly bool[] Levels;

        public bool IsEnabled;

        public LogConfig()
        {
            TargetConfigs = new TargetConfig[0];
            Levels = new bool[6];
        }

        public void AddTarget(int logLevel, Target target)
        {
            AddTarget(logLevel, logLevel, target);
        }

        public void AddTarget(int minLevel, int maxLevel, Target target)
        {
            var t = new TargetConfig[TargetConfigs.Length + 1];
            TargetConfigs.CopyTo(t, 0);
            TargetConfigs = t;

            TargetConfigs[TargetConfigs.Length - 1] = new TargetConfig(minLevel, maxLevel, target);
            for (int level = minLevel; level <= maxLevel; level++)
            {
                Levels[level] = true;
            }
        }

        public bool IsLevelEnabled(int logLevel)
        {
            return Levels[logLevel];
        }
    }
}

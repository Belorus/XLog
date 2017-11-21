using System.Collections.Generic;
using System.Linq;
using XLog.Categories;
using XLog.Formatters;

namespace XLog
{
    public class LogConfig
    {
        public readonly IFormatter Formatter;
        public readonly LogCategoryRegistrar CategoryRegistrar;

        internal List<TargetConfig> TargetConfigs;
        internal readonly bool[] Levels;

        public bool IsEnabled = true;

        public LogConfig(IFormatter formatter, LogCategoryRegistrar categoryRegistry = null)
        {
            Formatter = formatter;
            CategoryRegistrar = categoryRegistry ?? new LogCategoryRegistrar();
            TargetConfigs = new List<TargetConfig>();
            Levels = new bool[LogLevels.Levels.Length];
        }

        public void AddTarget(LogLevel logLevel, Target target)
        {
            AddTarget(logLevel, logLevel, target);
        }

        public void AddTarget(LogLevel minLevel, LogLevel maxLevel, Target target)
        {
            TargetConfigs.Add(new TargetConfig(minLevel, maxLevel, target));
            for (int level = (int)minLevel; level <= (int)maxLevel; level++)
            {
                Levels[level] = true;
            }
        }

        public bool IsLevelEnabled(int logLevel)
        {
            return Levels[logLevel];
        }

        public void Flush()
        {
            foreach (var target in TargetConfigs)
            {
                target.Target.Flush();
            }
        }
    }
}

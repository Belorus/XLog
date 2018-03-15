using System;
using System.Collections.Generic;
using XLog.Categories;
using XLog.Formatters;

namespace XLog
{
    public class LogConfig
    {
        public readonly IFormatter Formatter;
        public readonly LogCategoryRegistrar CategoryRegistrar;
        internal readonly List<TargetConfig> TargetConfigs;
        internal bool[] Levels;

        public bool IsEnabled = true;

        public LogConfig(IFormatter formatter, LogCategoryRegistrar categoryRegistry = null)
        {
            Formatter = formatter;
            CategoryRegistrar = categoryRegistry ?? new LogCategoryRegistrar();
            TargetConfigs = new List<TargetConfig>();
            Levels = new bool[LogLevels.Levels.Length];
        }

        public void Configure(Action<List<TargetConfig>> action)
        {
            action(TargetConfigs);
            
            UpdateLevels();
        }

        private void UpdateLevels()
        {
            var newLevelsArray = new bool[LogLevels.Levels.Length];

            foreach (var targetConfig in TargetConfigs)
            {
                for (int level = (int) targetConfig.MinLevel; level <= (int) targetConfig.MaxLevel; level++)
                {
                    newLevelsArray[level] = true;
                }
            }

            Levels = newLevelsArray;
        }

        [Obsolete("Use Configure")]
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

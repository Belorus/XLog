using System;
using System.Collections.Generic;
using System.Linq;

namespace XLog
{
    public class LogConfig
    {
        private readonly List<TargetConfig> _targetConfigs;
        private readonly List<Target> _targets;

        public bool IsEnabled;

        public LogConfig()
        {
            _targetConfigs = new List<TargetConfig>();
            _targets = new List<Target>();
        }

        public void AddTarget(int logLevel, Target target)
        {
            AddTarget(logLevel, logLevel, target);
        }

        public void AddTarget(int minLevel, int maxLevel, Target target)
        {
            _targetConfigs.Add(new TargetConfig(minLevel, maxLevel, target));
            _targets.Add(target);
        }

        internal IEnumerable<Target> GetTargets()
        {
            return _targets;
        }

        internal IEnumerable<Target> GetTargets(int logLevel)
        {
            return logLevel == LogLevel.Trace ? _targets : _targetConfigs.Where(c => c.SupportsLevel(logLevel)).Select(c => c.Target);
        }

        public bool IsLevelEnabled(int logLevel)
        {
            return _targetConfigs.Any(c => c.SupportsLevel(logLevel));
        }
    }
}

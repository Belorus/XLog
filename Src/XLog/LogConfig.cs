using System.Collections.Generic;

namespace XLog
{
    public class LogConfig
    {
        private TargetConfig[] _configs;
        private readonly bool[] _levels;
        private int _count;

        public bool IsEnabled;

        public LogConfig()
        {
            _configs = new TargetConfig[4];
            _levels = new bool[6];
        }

        public void AddTarget(int logLevel, Target target)
        {
            AddTarget(logLevel, logLevel, target);
        }

        public void AddTarget(int minLevel, int maxLevel, Target target)
        {
            if (_configs.Length == _count)
            {
                var t = new TargetConfig[_configs.Length * 2];
                _configs.CopyTo(t, 0);
                _configs = t;
            }

            _configs[_count] = new TargetConfig(minLevel, maxLevel, target);
            _count++;

            for (int level = minLevel; level <= maxLevel; level++)
            {
                _levels[level] = true;
            }
        }

        public bool IsLevelEnabled(int logLevel)
        {
            return _levels[logLevel];
        }

        internal IEnumerable<Target> GetTargets(int logLevel)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_configs[i].SupportsLevel(logLevel))
                {
                    yield return _configs[i].Target;
                }
            }
        }
    }
}

using System;
using System.Linq;

namespace XLog
{
    public class LogManager
    {
        private static LogManager _instance;

        public static LogManager Default
        {
            get { return _instance; }
        }

        public readonly LogConfig Config;
        
        private LogManager(LogConfig config)
        {
            Config = config;
        }

        public static void Init(LogConfig config)
        {
            _instance = new LogManager(config);

            string initMessage = string.Format(@"
>>> LogManager initialized successfully.
>>> UTC time: {0}
>>> Targets:
{1}",
                DateTime.UtcNow.ToString("R"),
                string.Join(
                    Environment.NewLine, 
                    config.TargetConfigs.Select(t => string.Format("\t'{0}' [{1} - {2}]", 
                                                                    t.Target.GetType().Name, 
                                                                    t.MinLevel, 
                                                                    t.MaxLevel))));

            _instance.GetLogger("XLog.LogManager").Info(initMessage);
        }

        public Logger GetLogger(string tag, LogConfig config = null)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
            }

            return new Logger(tag, config ?? Config);
        }

        public void Flush()
        {
            foreach (var target in Config.TargetConfigs)
            {
                target.Target.Flush();
            }
        }
    }
}

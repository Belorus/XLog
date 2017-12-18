using UnityEngine;
using XLog.Formatters;

namespace XLog.Unity
{
    public class UnityConsoleTarget : Target
    {
        public override void Write(Entry entry, IFormatter formatter)
        {
            switch (entry.Level)
            {
                case LogLevel.Trace:
                    Debug.Log(formatter.Format(entry));
                    break;
                case LogLevel.Debug:
                    Debug.Log(formatter.Format(entry));
                    break;
                case LogLevel.Info:
                    Debug.Log(formatter.Format(entry));
                    break;
                case LogLevel.Warn:
                    Debug.LogWarning(formatter.Format(entry));
                    break;
                case LogLevel.Error:
                    Debug.LogError(formatter.Format(entry));
                    break;
                case LogLevel.Fatal:
                    Debug.LogError(formatter.Format(entry));
                    break;
            }
        }

        public override void Write(string content)
        {
            throw new System.NotImplementedException();
        }
    }
}

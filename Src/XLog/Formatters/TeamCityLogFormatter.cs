using System;
using System.Text;

namespace XLog.Formatters
{
    public static class TeamCityLogUtils
    {
        public static string Escape(string what)
        {
            return what
                .Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\n", "|n")
                .Replace("\r", "|r")
                .Replace("[", "|[")
                .Replace("]", "|]");
        }

        public static string FlowIdClause(string flowId)
        {
            string flowIdClause = string.Empty;
            if (flowId != null)
            {
                flowIdClause = $"flowId='{flowId}'";
            }
            return flowIdClause;
        }
    }

    public class TeamCityLogFormatter : IFormatter
    {
        // Forgive me father for I have sinned
        private static readonly object MultilineLock = new object();

        private readonly string _flowIdClause;
        private readonly bool _doAsyncExceptionCleanup;

        public TeamCityLogFormatter(string flowId = null, bool doAsyncExceptionCleanup = true)
        {
            _doAsyncExceptionCleanup = doAsyncExceptionCleanup;
            _flowIdClause = TeamCityLogUtils.FlowIdClause(flowId);
        }

        public string Format(Entry entry)
        {
            var timestamp = $"{entry.TimeStamp:yyyy-MM-dd'T'HH:mm:ss.fff}{entry.TimeStamp.Ticks:+;-;}{entry.TimeStamp:hhmm}";

            string teamcityStatus;
            switch (entry.Level)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Info:
                    teamcityStatus = "NORMAL";
                    break;
                case LogLevel.Warn:
                    teamcityStatus = "WARNING";
                    break;
                case LogLevel.Error:
                    teamcityStatus = "ERROR";
                    break;
                case LogLevel.Fatal:
                    teamcityStatus = "FAILURE";
                    break;
                default:
                    teamcityStatus = "NORMAL";
                    break;
            }

            var errorDetails = string.Empty;
            if (entry.Exception != null)
            {
                errorDetails = entry.Exception.ToString();
                if (_doAsyncExceptionCleanup)
                {
                    errorDetails = ExceptionUtil.CleanStackTrace(errorDetails);
                }
            }

            var message = entry.Message;

            if (message.Contains("\n"))
            {
                var messageLines = message.Split(new[] { '\n' }, StringSplitOptions.None);
                var stringBuilder = new StringBuilder(message.Length + messageLines.Length + 1);
                lock (MultilineLock)
                {
                    foreach (var messageLine in messageLines)
                    {
                        var teamCityStatusMessage = TeamCityStatusMessage(_flowIdClause, messageLine, timestamp, errorDetails, teamcityStatus);
                        stringBuilder.AppendLine(teamCityStatusMessage);
                    }
                }

                return stringBuilder.ToString();
            }
            else
            {
                return TeamCityStatusMessage(_flowIdClause, message, timestamp, errorDetails, teamcityStatus);
            }
        }

        private static string TeamCityStatusMessage(
            string flowIdClause,
            string message,
            string timestamp,
            string errorDetails,
            string teamcityStatus)
        {
            // Flow id clause doesn't work when you use blocks, so I dunno what to do here other than not use it.
            //return $"##teamcity[message {flowIdClause} text='{Escape(message)}' timestamp='{timestamp}' errorDetails='{Escape(errorDetails)}' status='{teamcityStatus}']";
            return $"##teamcity[message text='{Escape(message)}' timestamp='{timestamp}' errorDetails='{Escape(errorDetails)}' status='{teamcityStatus}']";
        }

        private static string Escape(string str)
        {
            return TeamCityLogUtils.Escape(str);
        }
    }
}

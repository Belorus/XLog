using System;
using System.Text;

namespace XLog.Formatters
{
    public class TeamCityLogFormatter : IFormatter
    {
        // Forgive me father for I have sinned
        private static readonly object MultilineLock = new object();

        private readonly string _flowIdClause;

        public TeamCityLogFormatter(string flowId = null)
        {
            _flowIdClause = flowId != null ? $"flowId='{Escape(flowId)}'" : string.Empty;
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
                errorDetails = entry.Exception.Message;
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
            return $"##teamcity[message {flowIdClause} text='{Escape(message)}' timestamp='{timestamp}' errorDetails='{Escape(errorDetails)}' status='{teamcityStatus}']";
        }

        private static string Escape(string what)
        {
            return what
                .Replace("|", "||")
                .Replace("'", "|'")
                .Replace("\n", "|n")
                .Replace("\r", "|r")
                .Replace("[", "|[")
                .Replace("]", "|]");
        }
    }
}

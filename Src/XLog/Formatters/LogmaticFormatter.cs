namespace XLog.Formatters
{
    public class LogmaticFormatter : IFormatter
    {
        private readonly string _applicationName;
        private readonly string _hostName;
        private readonly string _apiKey;

        public LogmaticFormatter(
            string applicationName,
            string hostName,
            string apiKey)
        {
            _applicationName = applicationName;
            _hostName = hostName;
            _apiKey = apiKey;
        }

        public string Format(Entry entry)
        {
           
            int slot;
            var sb = FixedStringBuilderPool.Get(out slot);
            try
            {
                sb.Append(_apiKey);
                sb.Append(" {");

                sb.Append($"\"message\":\"{entry.Message}\"");
                sb.Append(",");
                sb.Append($"\"application\":\"{_applicationName}\"");
                sb.Append(",");
                sb.Append($"\"hostname\":\"{_hostName}\"");
                sb.Append(",");
                sb.Append($"\"level\":\"{entry.Level}\"");
                sb.Append(",");
                sb.Append($"\"date\":\"{entry.TimeStamp.ToString()}\"");
                sb.Append(",");
                sb.Append($"\"tag\":\"{entry.Tag}\"");

                sb.AppendLine("}");
                return sb.ToString();
            }
            finally
            {
                FixedStringBuilderPool.Return(slot, sb);
            }
        }
    }
}

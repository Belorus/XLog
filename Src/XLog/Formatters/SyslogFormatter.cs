using XLog.Categories;

namespace XLog.Formatters
{
    public class SyslogFormatter : IFormatter
    {
        private readonly string _uniqueId;
        private readonly string _applicationName;
        private readonly ICategoryFormatter _categoryFormatter;
        private readonly bool _doAsyncExceptionCleanup;

        public SyslogFormatter(
            string uniqueId,
            string applicationName,
            ICategoryFormatter categoryFormatter = null,
            bool doAsyncExceptionCleanup = true)
        {
            _uniqueId = uniqueId;
            _applicationName = applicationName;
            _categoryFormatter = categoryFormatter;
            _doAsyncExceptionCleanup = doAsyncExceptionCleanup;
        }

        public string Format(Entry entry)
        {
            int slotNumber;
            var builder = FixedStringBuilderPool.Get(out slotNumber);
            try
            {
                // This line looks strange but represents required format
                // http://help.papertrailapp.com/kb/configuration/configuring-remote-syslog-from-embedded-or-proprietary-systems/
                builder.Append($"<22>1 {entry.TimeStamp.ToString("s")} {_uniqueId} {_applicationName} - - - ");

                builder.Append(LogLevels.Levels[(int) entry.Level]);
                builder.Append("|");
                builder.Append(LogEnvironment.CurrentManagedThreadId.ToString());
                builder.Append("|");
                builder.Append(entry.Tag);
                builder.Append("|");

                if (_categoryFormatter != null)
                {
                    builder.Append(_categoryFormatter.GetString(entry.Category));
                    builder.Append("|");
                }

                builder.Append(entry.Message);
                if (entry.Exception != null)
                {
                    builder.Append(" --> ");
                    var exceptionString = entry.Exception.ToString();

                    if (_doAsyncExceptionCleanup)
                    {
                        exceptionString = ExceptionUtil.CleanStackTrace(exceptionString);
                    }

                    builder.Append(exceptionString);
                }

                builder.AppendLine();

                return builder.ToString();
            }
            finally
            {
                FixedStringBuilderPool.Return(slotNumber, builder);
            }
        }
    }
}

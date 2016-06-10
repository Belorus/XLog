using XLog.Categories;

namespace XLog.Formatters
{
    public class SlowLineFormatter : IFormatter
    {
        private readonly ICategoryFormatter _categoryFormatter;
        private readonly bool _doAsyncExceptionCleanup;

        public SlowLineFormatter(ICategoryFormatter categoryFormatter = null, bool doAsyncExceptionCleanup = true)
        {
            _categoryFormatter = categoryFormatter;
            _doAsyncExceptionCleanup = doAsyncExceptionCleanup;
        }

        public string Format(Entry entry)
        {
            int slotNumber;
            var builder = FixedStringBuilderPool.Get(out slotNumber);
            try
            {
                builder.Append(entry.TimeStamp.ToString("HH:mm:ss:fff"));
                builder.Append("|");
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

                return builder.ToString();
            }
            finally
            {
                FixedStringBuilderPool.Return(slotNumber, builder);
            }
        }
    }
}

using XLog.Categories;

namespace XLog.Formatters
{
    public enum LineEnding
    {
        None,
        CR,
        LF,
        CRLF
    }

    public class LineFormatter : IFormatter
    {
        private readonly ICategoryFormatter _categoryFormatter;
        private readonly bool _doAsyncExceptionCleanup;
        private readonly LineEnding _lineEnding;

        public LineFormatter(
            ICategoryFormatter categoryFormatter = null, 
            bool doAsyncExceptionCleanup = true,
            LineEnding lineEnding = LineEnding.CRLF)
        {
            _categoryFormatter = categoryFormatter;
            _doAsyncExceptionCleanup = doAsyncExceptionCleanup;
            _lineEnding = lineEnding;
        }

        public unsafe string Format(Entry entry)
        {
            int len = 25 + entry.Tag.Length + entry.Message.Length;

            // This fallback is needed because of possible huge stack allocation.
            if (len > 100*1024)
            {
                return FormatSlow(entry);
            }

            string categoryString = null;
            string exceptionString = null;

            if (_categoryFormatter != null)
            {
                categoryString = _categoryFormatter.GetString(entry.Category);

                len += categoryString.Length + 1;
            }

            if (entry.Exception != null)
            {
                exceptionString = entry.Exception.ToString();

                if (_doAsyncExceptionCleanup)
                {
                    exceptionString = ExceptionUtil.CleanStackTrace(exceptionString);
                }

                len += exceptionString.Length + 5;
            }

            char* charBuffer = stackalloc char[len];
            char* ptr = charBuffer;

            var dt = entry.TimeStamp;
            AppendDigitsFast(ref ptr, dt.Hour, 2);
            Append(&ptr, ':');
            AppendDigitsFast(ref ptr, dt.Minute, 2);
            Append(&ptr, ':');
            AppendDigitsFast(ref ptr, dt.Second, 2);
            Append(&ptr, ':');
            AppendDigitsFast(ref ptr, dt.Millisecond, 3);
            Append(&ptr, '|');

            Append(&ptr, LogLevels.Levels[(int) entry.Level]);

            Append(&ptr, '|');
            AppendDigitsFast(ref ptr, LogEnvironment.CurrentManagedThreadId, 2, ' ');
            Append(&ptr, '|');
            Append(&ptr, entry.Tag);
            Append(&ptr, '|');

            if (_categoryFormatter != null)
            {
                Append(&ptr, categoryString);
                Append(&ptr, '|');
            }

            Append(&ptr, entry.Message);

            if (entry.Exception != null)
            {
                Append(&ptr, " --> ");
                Append(&ptr, exceptionString);
            }

            switch (_lineEnding)
            {
                case LineEnding.CR:
                    Append(&ptr, '\r');
                    break;
                case LineEnding.LF:
                    Append(&ptr, '\n');
                    break;
                case LineEnding.CRLF:
                    Append(&ptr, '\r');
                    Append(&ptr, '\n');
                    break;
            }

            return new string(charBuffer, 0, len);
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static unsafe void Append(char** buffer, string s)
        {
            char* ptr = *buffer;
            for (int i = 0; i < s.Length; i++)
                *ptr++ = s[i];

            *buffer += s.Length;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static unsafe void Append(char** buffer, char c)
        {
            **buffer = c;
            *buffer += 1;
        }

#if !UNITY
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static unsafe void AppendDigitsFast(ref char* buffer, int value, int maxLen, char padding = '0')
        {
            char* p = buffer + maxLen;
            int n = value;
            do
            {
                *--p = (char)(n % 10 + '0');
                n /= 10;
            } while ((n != 0) && (p > buffer));

            int digits = (int)(buffer + maxLen - p);

            while ((digits < maxLen) && (p > buffer))
            {
                *--p = padding;
                digits++;
            }

            buffer += maxLen;
        }

        public string FormatSlow(Entry entry)
        {
            int slotNumber;
            var builder = FixedStringBuilderPool.Get(out slotNumber);
            try
            {
                var dt = entry.TimeStamp;
                builder.AppendDigitsFast(dt.Hour, 2);
                builder.Append(':');
                builder.AppendDigitsFast(dt.Minute, 2);
                builder.Append(':');
                builder.AppendDigitsFast(dt.Second, 2);
                builder.Append(':');
                builder.AppendDigitsFast(dt.Millisecond, 3);

                builder.Append("|");
                builder.Append(LogLevels.Levels[(int)entry.Level]);
                builder.Append("|");
                builder.AppendDigitsFast(LogEnvironment.CurrentManagedThreadId, 2, ' ');
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

                    string exceptionString = entry.Exception.ToString();
                    if (_doAsyncExceptionCleanup)
                    {
                        exceptionString = ExceptionUtil.CleanStackTrace(exceptionString);
                    }

                    builder.Append(exceptionString);
                }

                switch (_lineEnding)
                {
                    case LineEnding.CR:
                        builder.Append('\r');
                        break;
                    case LineEnding.LF:
                        builder.Append('\n');
                        break;
                    case LineEnding.CRLF:
                        builder.Append('\r');
                        builder.Append('\n');
                        break;
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

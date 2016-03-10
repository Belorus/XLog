using System;
using System.Runtime.CompilerServices;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        private readonly ICategoryResolver _categoryResolver;

        public LineFormatter(ICategoryResolver categoryResolver = null)
        {
            _categoryResolver = categoryResolver;
        }

        public unsafe string Format(Entry entry)
        {
            int len = 25 + entry.Tag.Length + entry.Message.Length;

            string categoryString = null;
            string exceptionString = null;

            if (_categoryResolver != null)
            {
                categoryString = _categoryResolver.GetString(entry.Category);

                len += categoryString.Length + 1;
            }

            if (entry.Exception != null)
            {
                exceptionString = entry.Exception.ToString();

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
            AppendDigitsFast(ref ptr, Environment.CurrentManagedThreadId, 2, ' ');
            Append(&ptr, '|');
            Append(&ptr, entry.Tag);
            Append(&ptr, '|');

            if (_categoryResolver != null)
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

            Append(&ptr, '\r');
            Append(&ptr, '\n');

            return new string(charBuffer, 0, len);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Append(char** buffer, string s)
        {
            char* ptr = *buffer;
            for (int i = 0; i < s.Length; i++)
                *ptr++ = s[i];

            *buffer += s.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Append(char** buffer, char c)
        {
            **buffer = c;
            *buffer += 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    }
}

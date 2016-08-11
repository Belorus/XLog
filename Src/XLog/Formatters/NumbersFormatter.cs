using System.Runtime.CompilerServices;
using System.Text;

namespace XLog.Formatters
{
    internal static unsafe class NumbersFormatter
    {
        public static void AppendDigitsFast(this StringBuilder builder, int value, int len, char padding = '0')
        {
            int maxLen = len;

            char* buffer = stackalloc char[maxLen];
            char* p = buffer + maxLen;
            int n = value;
            do
            {
                *--p = (char)(n % 10 + '0');
                n /= 10;
            } while ((n != 0) && (p > buffer));

            int digits = (int)(buffer + maxLen - p);

            //If the repeat count is greater than 0, we're trying
            //to emulate the "00" format, so we have to prepend
            //a zero if the string only has one character.
            while ((digits < len) && (p > buffer))
            {
                *--p = padding;
                digits++;
            }

            for (int i = 0; i < digits; ++i)
                builder.Append(p[i]);
        }

    }
}
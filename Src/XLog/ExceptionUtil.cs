using System.Text.RegularExpressions;

namespace XLog
{
    public static class ExceptionUtil
    {
        private const RegexOptions Options = RegexOptions.CultureInvariant;

        private static readonly Regex Pattern1 = new Regex(@"at System\.Runtime\.(?:Compiler|Exception)Services\.[^\n\r]+[\n\r\s]*", Options);
        private static readonly Regex Pattern2 = new Regex(@"--- End of stack trace from previous location where exception was thrown ---[\n\r]*", Options);
        private static readonly Regex Pattern3 = new Regex(@"in <filename unknown>:0", Options);
        private static readonly Regex Pattern4 = new Regex(@"\(code\.cs:1\)", Options);

        public static string CleanStackTrace(string textWithStackTrace)
        {
            // NOTE: Not very optimal but easy to read code. I assume that exceptions are not logged quite often
            //      Otherwise we have a bigger problem than slow logging :)

            string result = textWithStackTrace;
            result = Pattern1.Replace(result, string.Empty);
            result = Pattern2.Replace(result, string.Empty);
            result = Pattern3.Replace(result, string.Empty);
            result = Pattern4.Replace(result, string.Empty);
            return result;
        }
    }
}

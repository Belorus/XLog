using System;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        private const int NumbersCount = 100;
        private static readonly string[] Numbers;

        static LineFormatter()
        {
            Numbers = new string[NumbersCount];
            for (int i = 0; i < NumbersCount; i++)
            {
                Numbers[i] = i.ToString();
            }
        }

        public string Format(Entry entry)
        {
            var builder = StringBuilderPool.Get();
            builder.Append(entry.TimeStamp.ToString("HH:mm:ss:fff"));
            builder.Append("|");
            builder.Append(entry.LevelStr);
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId < NumbersCount ? Numbers[Environment.CurrentManagedThreadId] : Environment.CurrentManagedThreadId.ToString());
            builder.Append("|");
            builder.Append(entry.Tag);
            builder.Append("|");
            builder.Append(entry.Category);
            builder.Append("|");
            builder.Append(entry.Message);
            if (entry.Exception != null)
            {
                builder.Append(" --> ");
                builder.Append(entry.Exception);
            }

            return builder.ToString();
        }

    }
}

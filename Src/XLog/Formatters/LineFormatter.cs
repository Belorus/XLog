using System;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        private readonly ICategoryResolver _categoryResolver;

        public LineFormatter(ICategoryResolver categoryResolver = null)
        {
            _categoryResolver = categoryResolver;
        }

        public string Format(Entry entry)
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
                builder.Append(entry.LevelStr);
                builder.Append("|");
                builder.AppendDigitsFast(Environment.CurrentManagedThreadId, 2, ' ');
                builder.Append("|");
                builder.Append(entry.Tag);
                builder.Append("|");

                if (_categoryResolver != null)
                {
                    builder.Append(_categoryResolver.GetString(entry.Category));
                    builder.Append("|");
                }

                builder.Append(entry.Message);
                if (entry.Exception != null)
                {
                    builder.Append(" --> ");
                    builder.Append(entry.Exception);
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

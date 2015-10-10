using System;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        private const int NumbersCount = 100;
        private static readonly string[] Numbers;

        private readonly ICategoryResolver _categoryResolver;

        static LineFormatter()
        {
            Numbers = new string[NumbersCount];
            for (int i = 0; i < NumbersCount; i++)
            {
                Numbers[i] = i.ToString();
            }
        }

        public LineFormatter(ICategoryResolver categoryResolver = null)
        {
            _categoryResolver = categoryResolver;
        }

        public string Format(Entry entry)
        {
            int slotNumber;
            var builder = FixedStringBuilderPool.Get(out slotNumber);
            builder.Append(entry.TimeStamp.ToString("HH:mm:ss:fff"));
            builder.Append("|");
            builder.Append(entry.LevelStr);
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId < NumbersCount ? Numbers[Environment.CurrentManagedThreadId] : Environment.CurrentManagedThreadId.ToString());
            builder.Append("|");
            builder.Append(entry.Tag);
            builder.Append("|");
            
            if (_categoryResolver != null)
            {
                builder.Append(entry.Category);
                builder.Append("|");
            }

            builder.Append(entry.Message);
            if (entry.Exception != null)
            {
                builder.Append(" --> ");
                builder.Append(entry.Exception);
            }

            var str = builder.ToString();

            FixedStringBuilderPool.Return(slotNumber, builder);

            return str;
        }

    }
}

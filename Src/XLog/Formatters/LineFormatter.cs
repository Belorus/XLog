using System;
using System.Text;
using System.Threading;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            int index = Interlocked.Increment(ref _index);
            var builder = _builders[index % BuildersCount];
            builder.Clear();
            builder.Append(entry.TimeStamp.ToString("HH:mm:ss:fff"));
            builder.Append("|");
            builder.Append(entry.LevelStr);
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId < 100 ? Numbers[Environment.CurrentManagedThreadId]: Environment.CurrentManagedThreadId.ToString());
            builder.Append("|");
            builder.Append(entry.Tag);
            builder.Append("|");
            builder.Append(entry.Message);
            if (entry.Exception != null)
            {
                builder.Append(" --> ");
                builder.Append(entry.Exception);
            }

            return builder.ToString();
        }

        private int _index = -1;
        private const int BuildersCount = 10;
        private const int BuilderLength = 1000;
        private readonly StringBuilder[] _builders =
        {
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
            new StringBuilder(BuilderLength),
        };

        private static readonly string[] Numbers =
        {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60",
            "61",
            "62",
            "63",
            "64",
            "65",
            "66",
            "67",
            "68",
            "69",
            "70",
            "71",
            "72",
            "73",
            "74",
            "75",
            "76",
            "77",
            "78",
            "79",
            "80",
            "81",
            "82",
            "83",
            "84",
            "85",
            "86",
            "87",
            "88",
            "89",
            "90",
            "91",
            "92",
            "93",
            "94",
            "95",
            "96",
            "97",
            "98",
            "99",
        };
    }
}

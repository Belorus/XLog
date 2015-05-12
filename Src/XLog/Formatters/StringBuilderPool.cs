using System;
using System.Text;
using System.Threading;

namespace XLog.Formatters
{
    public static class StringBuilderPool
    {
        private const int BuildersCount = 10;
        private const int BuilderLength = 1000;
        private static readonly StringBuilder[] Builders;
        private static int _index = -1;

        static StringBuilderPool()
        {
            Builders = new StringBuilder[BuildersCount];
            for (int i = 0; i < BuildersCount; i++)
            {
                Builders[i] = new StringBuilder(BuilderLength);
            }
        }

        public static StringBuilder Get()
        {
            int index = Interlocked.Increment(ref _index);
            var builder = Builders[index % BuildersCount];
            builder.Clear();
            return builder;
        }
    }
}

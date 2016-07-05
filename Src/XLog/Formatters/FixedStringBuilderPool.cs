using System.Text;
using System.Threading;

namespace XLog.Formatters
{
    public static class FixedStringBuilderPool
    {
        private const int BuildersCount = 7;
        private const int BuilderInitialLength = 500;
        private const int BuilderMaxLength = 5000;

        private static readonly StringBuilder[] Builders;
        private static int _index = -1;

        static FixedStringBuilderPool()
        {
            Builders = new StringBuilder[BuildersCount];
            for (int i = 0; i < BuildersCount; i++)
            {
                Builders[i] = new StringBuilder(BuilderInitialLength);
            }
        }

        public static StringBuilder Get(out int num)
        {
            // If all slots in array == null - the thread will spin until one is returned
            // I hope this situation will never happen. 7 threads concurrently writing logs on mobile platform is something very unusual
            int index;
            StringBuilder builder;
            do
            {
                index = Interlocked.Increment(ref _index) % BuildersCount;
            } while ((builder = Interlocked.Exchange(ref Builders[index], null)) == null);

            num = index;
            return builder;
        }

        public static void Return(int num, StringBuilder sb)
        {
            if (sb.Capacity > BuilderMaxLength)
            {
                sb.Length = 0;
                sb.Capacity = BuilderMaxLength;
            }
            else
            {
                sb.Length = 0;
            }

            Builders[num] = sb;

        }
    }
}

using System.Text;
using System.Threading;

namespace XLog.Formatters
{
    internal static class FixedStringBuilderPool
    {
        private const int BuildersCount = 5;
        private const int BuilderLength = 200;
        private static readonly StringBuilder[] Builders;
        private static int _index = -1;

        static FixedStringBuilderPool()
        {
            Builders = new StringBuilder[BuildersCount];
            for (int i = 0; i < BuildersCount; i++)
            {
                Builders[i] = new StringBuilder(BuilderLength);
            }
        }

        public static StringBuilder Get(out int num)
        {
            // If there all slots in array == null - the thread will spin until one is returned
            // I hope this situation will never happen. 5 threads concurrently writing logs on mobile platforms is something very unusual
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
            sb.Clear();
            Builders[num] = sb;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace XLog.Sample.Console
{
    internal static class Logic
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Logic");

        public static void TestWrite()
        {
            var tasks = new List<Task>();
            var sw = Stopwatch.StartNew();
            for (var j = 0; j < 2; j++)
            {
                var task = Task.Run(() =>
                {
                    for (var i = 0; i < 100000; i++)
                    {
                        Log.Info("Hello world!");
                    }
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            sw.Stop();
            System.Console.WriteLine(sw.ElapsedMilliseconds + " ms");
        }
    }
}
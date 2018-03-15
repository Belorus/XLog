﻿using System;
using System.Threading.Tasks;
using XLog.Categories;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.Console
{
    internal class Program
    {
        private static void Main()
        {
            InitLogger();

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            try
            {
                Logic.TestCategory();
            }
            catch (Exception ex)
            {
                LogManager.Default.GetLogger("CrashLogger").Error("oops", ex);
                throw;
            }
            finally
            {
                LogManager.Default.Flush();
            }

            System.Console.ReadLine();
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogManager.Default.GetLogger("UnobservedCrashLogger").Error("oops", e.Exception);
        }

        private static void InitLogger()
        {
            var categoryRegistry = new LogCategoryRegistrar();
            var formatter = new LineFormatter(new DefaultCategoryFormatter(categoryRegistry));
            var logConfig = new LogConfig(formatter, categoryRegistry);

            logConfig.Configure(list => list.Add(new TargetConfig(LogLevel.Trace, LogLevel.Fatal, new ConsoleTarget())));

            LogManager.Init(logConfig);
        }
    }
}
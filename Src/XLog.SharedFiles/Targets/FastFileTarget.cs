using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class FastFileTarget : Target, ILogStorage
    {
        private readonly FileStream _file;
        private readonly BlockingCollection<string> _collection;
        private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);

        private readonly string _logFileDirectory;

        private bool _disposed;
        private readonly string _logFilePath;

        public FastFileTarget(IFormatter formatter, string logFilePath)
            : base(formatter)
        {
            _logFilePath = logFilePath;
            _logFileDirectory = Path.GetDirectoryName(logFilePath);

            Directory.CreateDirectory(_logFileDirectory);

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            _file = File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            _collection = new BlockingCollection<string>();
            Start();
        }

        public override void Flush()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _collection.CompleteAdding();
            _completeEvent.WaitOne();
        }

        private async void Start()
        {
            try
            {
                await Task.Factory.StartNew(RunConsumer, TaskCreationOptions.LongRunning);
            }
            finally
            {
                _completeEvent.Set();
            }
        }

        private void RunConsumer()
        {
            using (var writer = new StreamWriter(_file, Encoding.UTF8))
            {
                foreach (var s in _collection.GetConsumingEnumerable())
                {
                    writer.Write(s);
                }
            }
        }

        public override void Write(string content)
        {
            if (_disposed)
            {
                return;
            }

            _collection.Add(content);
        }

        public string GetLastLogs()
        {
            Flush();

            var file = new FileInfo(_logFilePath);

            int numOfRetries = 3;
            do
            {
                try
                {
                    return ReadFileContentsSafe(file);
                }
                catch (IOException)
                {
                }
            } while (--numOfRetries > 0);

            return string.Empty;
        }

        private static string ReadFileContentsSafe(FileInfo f)
        {
            string copyName = f.FullName + ".copy";

            File.Copy(f.FullName, copyName);
            try
            {
                using (var fileStream = File.OpenText(copyName))
                {
                    return fileStream.ReadToEnd();
                }
            }
            finally
            {
                File.Delete(copyName);
            }
        }
    }
}

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

        private readonly string _logFilePath;
        private readonly string _logFileDirectory;

        private bool _disposed;

        public FastFileTarget(IFormatter formatter, string logFilePath)
            : base(formatter)
        {
            _logFileDirectory = Path.GetDirectoryName(logFilePath);
            _logFilePath = logFilePath;

            Directory.CreateDirectory(_logFileDirectory);

            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }

            _file = File.Open(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
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

        public byte[][] GetLastLogs(int count)
        {
            FileInfo[] logFiles = Directory.GetFiles(_logFileDirectory).Select(f => new FileInfo(f)).OrderByDescending(x => x.CreationTime).Take(count).ToArray();

            byte[][] logsContent = null;
            do
            {
                try
                {
                    logsContent = logFiles.Select(ReadFileContentsSafe).ToArray();
                }
                catch (IOException)
                {

                }
            } while (logsContent == null);

            return logsContent;
        }

        private static byte[] ReadFileContentsSafe(FileInfo f)
        {
            string copyName = f.FullName + ".copy";
            byte[] bytes = new byte[f.Length];

            File.Copy(f.FullName, copyName);
            using (var stream = File.OpenRead(copyName))
                stream.Read(bytes, 0, bytes.Length);
            File.Delete(copyName);

            return bytes;
        }

    }
}

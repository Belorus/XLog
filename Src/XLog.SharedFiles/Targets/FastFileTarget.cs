using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XLog.NET.Targets
{
    public class FastFileTarget : Target, IDisposable, IFileTarget
    {
        private readonly FileStream _file;
        private readonly BlockingCollection<string> _collection;
        private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);

        public readonly string FileNamePrefix;
        private bool _disposed;

        public FastFileTarget(IFormatter formatter, string path, string fileNamePrefix)
            : base(formatter)
        {
            Path = path;
            FileNamePrefix = fileNamePrefix;

            Directory.CreateDirectory(Path);

#if DEV_BUILD
            var fileName = System.IO.Path.Combine(Path, FileNamePrefix) + DateTime.Now.ToString("s").Replace(":", "_") + ".log";
#else
            var fileName = System.IO.Path.Combine(Path, FileNamePrefix) + ".log";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
#endif

            _file = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _collection = new BlockingCollection<string>();
            Start();
        }

        public string Path { get; private set; }

        public void Dispose()
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
                    writer.WriteLine(s);
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

        public byte[][] CollectLastLogs(int count)
        {
            FileInfo[] logFiles = Directory.GetFiles(Path).Select(f => new FileInfo(f)).OrderByDescending(x => x.CreationTime).Take(count).ToArray();

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

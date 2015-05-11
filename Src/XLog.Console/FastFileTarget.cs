using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XLog
{
    public class FastFileTarget : Target, IDisposable
    {
        private readonly FileStream _file;
        private readonly BlockingCollection<string> _collection;

        public readonly string Path;
        public readonly string FileNamePrefix;
        private bool _disposed;

        public FastFileTarget(string path, string fileNamePrefix)
            : this(null, path, fileNamePrefix)
        {
        }

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

        public void Dispose()
        {
            _disposed = true;
            _collection.CompleteAdding();
        }

        private async void Start()
        {
            await Task.Factory.StartNew(RunConsumer, TaskCreationOptions.LongRunning);
        }

        private void RunConsumer()
        {
            using (var writer = new StreamWriter(_file, Encoding.UTF8))
            {
                foreach (var s in _collection.GetConsumingEnumerable())
                {
                    var contents = s;
                    if (contents.Length > 5000)
                    {
                        contents = ">>>>>> " + contents.Replace(Environment.NewLine, string.Empty);
                    }

                    writer.WriteLine(contents);
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
    }
}

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace XLog
{
    public class SyncFileTarget : Target
    {
        private readonly object _syncRoot = new object();

        public SyncFileTarget(string path, string fileNamePrefix)
            : this(null, path, fileNamePrefix)
        {
        }

        public SyncFileTarget(IFormatter formatter, string path, string fileNamePrefix)
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

            var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(file, Encoding.UTF8);
        }

        public string Path { get; private set; }
        public string FileNamePrefix;
        private readonly StreamWriter _writer;

        public override void Write(string content)
        {
            lock (_syncRoot)
            {
                _writer.WriteLine(content);
            }
        }

        public byte[][] CollectLastLogs(int count)
        {
            lock (_syncRoot)
            {
                _writer.Flush();

                FileInfo[] logFiles = Directory.GetFiles(Path)
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(x => x.CreationTime)
                    .Take(count)
                    .ToArray();

                byte[][] logsContent = new byte[count][];

                for (int i = 0; i < logFiles.Length; i++)
                {
                    do
                    {
                        try
                        {
                            logsContent[i] = ReadFileContentsSafe(logFiles[i]);
                        }
                        catch (IOException)
                        {
                        }
                    } while (logsContent[i] == null);
                }

                return logsContent;
            }
        }

        private static byte[] ReadFileContentsSafe(FileInfo f)
        {
            string copyName = f.FullName + ".copy";
            byte[] bytes = new byte[f.Length];
            File.Copy(f.FullName, copyName);
            using (var stream = File.OpenRead(copyName))
            {
                stream.Read(bytes, 0, bytes.Length);
            }

            File.Delete(copyName);

            return bytes;
        }

        public override void Flush()
        {
            lock (_syncRoot)
            {
                _writer.Flush();
            }
        }
    }
}

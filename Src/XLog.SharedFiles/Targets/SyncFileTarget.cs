using System;
using System.IO;
using System.Linq;
using System.Text;

namespace XLog.NET.Targets
{
    public class SyncFileTarget : Target, IFileTarget
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

            try
            {
                var file = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                _writer = new StreamWriter(file, Encoding.UTF8);
            }
            catch (IOException)
            {
                _writer = StreamWriter.Null;
            }
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
                Flush();

                FileInfo[] logFiles = Directory.GetFiles(Path)
                                               .Select(f => new FileInfo(f))
                                               .OrderByDescending(x => x.CreationTime)
                                               .Take(count)
                                               .ToArray();

                byte[][] logsContent = new byte[logFiles.Length][];

                for (int i = 0; i < logFiles.Length; i++)
                {
                    int numOfRetries = 3;
                    do
                    {
                        try
                        {
                            logsContent[i] = ReadFileContentsSafe(logFiles[i]);
                        }
                        catch (IOException)
                        {
                        }
                    } while (logsContent[i] == null && --numOfRetries > 0);
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
                try
                {
                    _writer.Flush();
                }
                catch (IOException)
                {
                    // If log file cannot be flushed - we shouldn't crash. 
                    // Supressing finalization to avoid crash in finalizer
                    GC.SuppressFinalize(_writer);
                    GC.SuppressFinalize(_writer.BaseStream);
                }
            }
        }
    }
}

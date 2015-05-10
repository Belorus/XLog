using System;
using System.IO;
using System.Linq;
using System.Text;

namespace XLog
{
    public class SyncFileTarget : Target
    {
        private readonly FileStream _file;
        private readonly object _syncRoot;

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

            _file = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _syncRoot = new object();
        }

        public string Path { get; private set; }
        public string FileNamePrefix;


        public override void Write(Entry entry)
        {
            var contents = Formatter.Format(entry);

            lock (_syncRoot)
            {
                try
                {
                    using (var writer = new StreamWriter(_file, Encoding.UTF8, 4096, true))
                    {
                        if (contents.Length > 5000)
                        {
                            contents = ">>>>>> " + contents.Replace(Environment.NewLine, string.Empty);
                        }

                        writer.WriteLine(contents);
                    }
                }
                catch (IOException)
                {
                }
            }
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
            {
                stream.Read(bytes, 0, bytes.Length);
            }

            File.Delete(copyName);

            return bytes;
        }
    }
}

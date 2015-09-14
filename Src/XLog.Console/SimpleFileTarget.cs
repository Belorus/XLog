using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace XLog.NET
{
    public class SimpleFileTarget : Target, IFileTarget
    {
        private readonly AsyncSemaphore _semaphore = new AsyncSemaphore(1);
        private string _fileName;
        private FileStream _file;

        public string Path { get; private set; }
        public string FileNamePrefix { get; private set; }

        public string FileName
        {
            get { return _fileName; }
        }

        public SimpleFileTarget(IFormatter layout, string path, string fnamePrefix) 
            : base(layout)
        {
            Path = path;
            FileNamePrefix = fnamePrefix;

            Directory.CreateDirectory(Path);

#if DEV_BUILD
            _fileName = System.IO.Path.Combine(Path, FileNamePrefix) + DateTime.Now.ToString("s").Replace(":", "_") + ".log";
#else
            _fileName = System.IO.Path.Combine(Path, FileNamePrefix) + ".log";
            if (File.Exists(_fileName))
                File.Delete(_fileName);
#endif

            _file = File.Open(_fileName, FileMode.Append, FileAccess.Write, FileShare.Read);            
        }

        public override async void Write(string content)
        {
            await _semaphore.WaitAsync();

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i > 0)
                        await Task.Delay(20);

                    try
                    {
                        using (var writer = new StreamWriter(_file, Encoding.UTF8, 4096, true))
                        {
                            writer.WriteLine(content);
                        }

                        break;
                    }
                    catch (IOException)
                    {

                    }
                }
            }
            finally
            {
                _semaphore.Release();
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
                stream.Read(bytes, 0, bytes.Length);
            File.Delete(copyName);

            return bytes;
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace XLog.Sample.Console
{
    public class InMemoryBufferTarget : Target
    {
        private readonly MemoryStream _stream;
        private readonly byte[] _buffer;
        private readonly Encoding _encoding;

        public InMemoryBufferTarget(int sizeInBytes)
        {
            _stream = new MemoryStream(sizeInBytes);
            _buffer = new byte[sizeInBytes];
            _encoding = Encoding.UTF8;
        }

        public override void Write(string content)
        {
            var len = _encoding.GetBytes(content, 0, content.Length, _buffer, 0);

            var outOfScope = Math.Max(0, (int) (_stream.Position + len - _stream.Capacity));
            if (outOfScope > 0)
            {
                var inScope = len - outOfScope;

                _stream.Write(_buffer, 0, inScope);
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.Write(_buffer, inScope, outOfScope);
            }
            else
            {
                _stream.Write(_buffer, 0, len);
            }
        }

        public string GetContents()
        {
            byte[] buffer = new byte[_stream.Capacity];

            int position = (int) _stream.Position;
            int tailLen = (int) (_stream.Length - _stream.Position);

            _stream.Read(buffer, 0, tailLen);
            _stream.Seek(0, SeekOrigin.Begin);
            if (position > 0)
                _stream.Read(buffer, tailLen, position);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
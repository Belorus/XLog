using System;
using System.Text;

namespace XLog.NET.Targets
{
    public class InMemoryBufferTarget : Target, ILogStorage
    {
        private readonly char[] _buffer;
        private int _curPos;

        public InMemoryBufferTarget(int sizeInBytes)
        {
            _buffer = new char[sizeInBytes];
        }

        public override void Write(string content)
        {
            var len = content.Length;

            var outOfScope = Math.Max(0, (_curPos + len - _buffer.Length));
            if (outOfScope > 0)
            {
                var inScope = len - outOfScope;

                content.CopyTo(0, _buffer, _curPos, inScope);
                content.CopyTo(inScope, _buffer, 0, outOfScope);
                _curPos = outOfScope;
            }
            else
            {
                content.CopyTo(0, _buffer, _curPos, content.Length);
                _curPos += len;
            }
        }

        public string GetContents()
        {
            return new string(_buffer, _curPos, _buffer.Length - _curPos) + new string(_buffer, 0, _curPos);
        }

        public byte[][] GetLastLogs(int count)
        {
            string text = GetContents();

            return new[] {Encoding.UTF8.GetBytes(text)};
        }
    }
}
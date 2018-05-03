using System;
using System.Collections.Generic;
using System.Threading;

namespace XLog.NET.Targets.LogRoom
{
    public class BufferingDispatcher<T>
    {
        public delegate void ProcessAsyncDelegate(IList<T> items, Action<bool> callback);

        private readonly Queue<T> _queue = new Queue<T>();
        private readonly ProcessAsyncDelegate _sender;
        private readonly object _syncRoot = new object();
        private Timer _timer;

        public BufferingDispatcher(ProcessAsyncDelegate processDelegate)
        {
            _sender = processDelegate;
        }

        public TimeSpan FlushDelay { get; set; } = TimeSpan.FromSeconds(10);

        public int MaxQueueSize { get; set; } = 10;

        public void Dispatch(T eventToSend)
        {
            bool isQueueFull;
            lock (_syncRoot)
            {
                _queue.Enqueue(eventToSend);
                isQueueFull = _queue.Count >= MaxQueueSize;
            }

            if (isQueueFull)
                Flush();
            else
                EnsureTimerRuns();
        }

        private void EnsureTimerRuns()
        {
            lock (_syncRoot)
            {
                if (_timer == null)
                {
                    _timer = new Timer(_ => Flush(), null, (int)FlushDelay.TotalMilliseconds, 0);
                }
            }
        }

        private void Flush()
        {
            T[] array;
            lock (_syncRoot)
            {
                _timer?.Dispose();
                _timer = null;

                array = _queue.ToArray();
                _queue.Clear();
            }

            _sender(array, isSuccess => { });
        }
    }
}

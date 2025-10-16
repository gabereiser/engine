using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Red.Common;
using Red.Platform;
using Timer = System.Timers.Timer;

namespace Red
{
    public class Threading : Singleton<Threading>, IDisposable
    {
        private Timer statTimer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
        private static readonly Thread mainThread = Thread.CurrentThread;
        private static readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();
        public Threading()
        {
            statTimer.Elapsed += (o, e) => { Metrics.ProcessMetrics(); };
            statTimer.Start();
        }

        public void Dispose()
        {
            statTimer.Stop();
        }

        public static void Flush()
        {
            if (queue.Count > 0)
            {
                while (queue.Count > 0)
                {
                    if (queue.TryDequeue(out var action))
                        action.Invoke();
                }
            }
        }
        public static void RunOnMain(Action action)
        {
            if (Thread.CurrentThread != mainThread)
            {
                queue.Enqueue(action);
                return;
            }

            action.Invoke();
        }

        public static void RunOnPool(Action action)
        {
            Task.Factory.StartNew(action);

        }

        public static void Queue(Action action)
        {
            RunOnPool(action);
        }

        public static Thread RunOnHardware(Action action)
        {
            var t = new Thread(() =>
            {
                action.Invoke();
            });
            t.Start();
            return t;
        }
    }

}
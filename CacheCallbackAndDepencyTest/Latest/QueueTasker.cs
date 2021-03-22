using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HybridServer
{
    internal class QueueTasker
    {
        private readonly LimitedConcurrencyLevelTaskScheduler lcts;
        private readonly List<Task> tasks;
        private readonly TaskFactory factory;
        private readonly CancellationTokenSource cts;
#pragma warning disable
        private readonly object lockObj;
#pragma warning restore
        internal QueueTasker()
        {
            lcts = new LimitedConcurrencyLevelTaskScheduler(1);
            tasks = new List<Task>();
            factory = new TaskFactory(lcts);
            cts = new CancellationTokenSource();
            lockObj = new object();
        }
        internal Task Add(Action action)
        {
            var task = factory.StartNew(action, cts.Token);
            tasks.Add(task);
            return task;
        }
    }
}
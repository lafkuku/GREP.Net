using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amib.Threading;
using NLog;

namespace Grep.Net.Model.Types
{
    public class ActionScheduler : TaskScheduler, IDisposable
    {
        //Attributes  
        /// <summary>Whether the current thread is processing work items.</summary> 
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems; 

        private ConcurrentQueue<Task> _tasks;
        private object _lok = new object(); 

        private int _maxConcurrency;
        private int _currentConcurrency;

        private SmartThreadPool _threadPool; 

        //Properties
        public bool Working { get; set; }

        public int MaxConcurrency
        {
            get
            {
                return _maxConcurrency;
            }
            set
            {
                _maxConcurrency = value;
                _threadPool.MaxThreads = value;
            }
        }

        public int CurrentConcurrency
        {
            get
            {
                return _currentConcurrency;
            }
            set
            {
                _currentConcurrency = value;
            }
        }

        public int TaskCount
        {
            get
            {
                return _tasks.Count;
            }
        }

        //Logger
        public static Logger logger = LogManager.GetCurrentClassLogger(); 

        public ActionScheduler(int maxConcurrency = 25, bool startOnInit = true)
        {
            _maxConcurrency = maxConcurrency;
            _currentConcurrency = 0;
            _tasks = new ConcurrentQueue<Task>();
            STPStartInfo startConfig = new STPStartInfo();

            _threadPool = new SmartThreadPool();
            _threadPool.MaxThreads = maxConcurrency;
            Working = true;

            if (startOnInit)
                Start();
        }

        public void Start()
        {
            Thread t = new Thread((ParameterizedThreadStart)DoWork);
            Working = true;
            _threadPool.Start(); 
            t.Start();
        }

        public void Stop()
        {
            this.Working = false;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasks;
        }
        protected override void QueueTask(Task task)
        {
            _tasks.Enqueue(task);
        }
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining 
            if (!_currentThreadIsProcessingItems)
                return false;

            // If the task was previously queued, remove it from the queue 
            if (taskWasPreviouslyQueued)
                TryDequeue(task);

            // Try to run the task. 
            return base.TryExecuteTask(task); 
        }
        private void DoWork(object param)
        {
            // Note that the current thread is now processing work items. 
            // This is necessary to enable inlining of tasks into this thread. 
            _currentThreadIsProcessingItems = true;
            
            while (Working)
            {
                if (CurrentConcurrency < MaxConcurrency)
                {
                    Task t = null;
                    if (_tasks.TryDequeue(out t))
                    {
                        /*lock (_lok)*/
                        _currentConcurrency++;
                        try
                        {
                            _threadPool.QueueWorkItem(() =>
                            {
                                try
                                {
                                    base.TryExecuteTask(t);
                                    //logger.Info("Task completed on MThreadID: " + Thread.CurrentThread.ManagedThreadId);
                                }
                                catch (Exception e2)
                                {
                                    logger.Error(e2);
                                }
                                finally
                                {
                                    /*lock (_lok)*/
                                    _currentConcurrency--;
                                }
                            });
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                            _currentConcurrency--;
                        }
                    }
                }
                Thread.Sleep(10); 
            }
        }
        public void Dispose()
        {
            this.Stop();
            this._threadPool.Dispose();
        }
    }
}
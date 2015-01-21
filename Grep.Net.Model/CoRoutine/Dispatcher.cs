using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Caliburn.Micro;

namespace Grep.Net.Model.CoRoutine
{
    public class Dispatcher : IDisposable
    {
        #region Singleton
        
        private static Dispatcher _instance;
        
        public static Dispatcher Instance
        {
            get
            {
                lock (_lok)
                {
                    if (_instance == null)
                    {
                        _instance = new Dispatcher();
                    }
                }
                return _instance;
            }
        }
        
        private static object _lok = new object();
        
        #endregion
        
        public bool Running { get; set; }
        
        public Queue<IResult> CommandQueue { get; set; }
        
        private bool Shutdown { get; set; }
        
        public bool Initialized { get; private set; }
        
        private Thread _dispatcherThread { get; set; }
        
        public Dispatcher()
        {
            Running = true;
            CommandQueue = new Queue<IResult>(); 
            StartInternal();
            Initialized = false;
        }
        
        public void QueueItem(IResult coroutine)
        {
            if (coroutine == null)
                throw new ArgumentException("Null coroutine passed to the dispatcher"); 
            
            this.CommandQueue.Enqueue(coroutine); 
        }
        
        public void StartInternal()
        {
            if (!Initialized)
            {
                _dispatcherThread = new Thread(() =>
                {
                    while (true)
                    {
                        if (Running)
                        {
                            if (CommandQueue.Count > 0)
                            {
                                IResult nextItem = CommandQueue.Dequeue();
                                
                                ThreadPool.QueueUserWorkItem(new WaitCallback((x) =>
                                {
                                    CoroutineExecutionContext context = null;
                                    if (x is CoroutineExecutionContext)
                                    {
                                        context = x as CoroutineExecutionContext;
                                    }
                                    nextItem.Execute(context);
                                }));
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
                        
                        if (Shutdown)
                        {
                            break;
                        }
                    }
                });
                
                _dispatcherThread.Start(); 
            }
        }
        
        public void Dispose()
        {
            //We could shutdown, but lets just cheat.. 
            _dispatcherThread.Abort(); 
        }
    }
}
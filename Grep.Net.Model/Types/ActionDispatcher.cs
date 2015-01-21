using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grep.Net.Model.Types
{
    public class ActionDispatcher
    {
        public ActionScheduler Scheduler { get; set; }

        public ActionDispatcher()
        {
            this.Scheduler = new ActionScheduler(); 
        }

        public ActionDispatcher(ActionScheduler scheduler)
        {
            this.Scheduler = scheduler;
        }

        public void Dispatch(Action a)
        {
            var cToken = CancellationToken.None;
            Task.Factory.StartNew(a, cToken, TaskCreationOptions.None, Scheduler);  
        }

        public void Stop()
        {
            this.Scheduler.Stop();
        }
    }
}
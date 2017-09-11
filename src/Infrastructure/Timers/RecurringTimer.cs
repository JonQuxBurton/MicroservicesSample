using System;
using System.Threading;

namespace Infrastructure.Timers
{
    public class RecurringTimer : IRecurringTimer
    {
        public event EventHandler Target;
        private Timer timer;
        private readonly int delay;
        private readonly int interval;

        public RecurringTimer(int delay, int interval)
        {
            this.delay = delay;
            this.interval = interval;
        }

        public void Start()
        {
            var autoEvent = new AutoResetEvent(false);

            this.timer = new Timer(TimeUp, autoEvent, this.delay, this.interval);
        }

        private void TimeUp(object stateInfo)
        {
            Target.Invoke(stateInfo, new EventArgs());
        }
    }
}

using System;

namespace Infrastructure.Timers
{
    public interface IRecurringTimer
    {
        event EventHandler Target;
        void Start();
    }
}

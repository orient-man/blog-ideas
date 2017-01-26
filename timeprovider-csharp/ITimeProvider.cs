using System;

namespace App.Common.Calendar
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }

        void Wait(TimeSpan timeToWait);
    }
}
using System;
using System.Threading;

namespace App.Common.Calendar
{
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;

        public void Wait(TimeSpan timeToWait)
        {
            Thread.Sleep(timeToWait);
        }
    }
}
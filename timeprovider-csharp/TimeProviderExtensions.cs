using System;

namespace App.Common.Calendar
{
    public static class TimeProviderExtensions
    {
        public static void Wait(this ITimeProvider @this, int timeToWaitInMilliseconds)
        {
            @this.Wait(TimeSpan.FromMilliseconds(timeToWaitInMilliseconds));
        }
    }
}
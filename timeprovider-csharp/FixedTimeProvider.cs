using System;
using App.Common.Calendar;

namespace App.Testing.Infrastructure
{
    public class FixedTimeProvider : ITimeProvider, IDisposable
    {
        private readonly ITimeProvider _originalTimeProvider;

        public DateTime Now { get; private set; }

        public DateTime Today => Now.Date;

        public FixedTimeProvider() : this(DateTime.Now)
        {
        }

        public FixedTimeProvider(DateTime dateTime)
        {
            Now = dateTime;
            _originalTimeProvider = TimeProvider.Current;
            TimeProvider.SetTimeProviderForTesting(this);
        }

        public virtual void Wait(TimeSpan timeToWait)
        {
            Now = Now.Add(timeToWait);
        }

        public void ChangeTime(DateTime datetime)
        {
            Now = datetime;
        }

        public void Dispose()
        {
            TimeProvider.SetTimeProviderForTesting(_originalTimeProvider);
        }
    }
}
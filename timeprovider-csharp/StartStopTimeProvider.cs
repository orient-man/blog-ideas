using System;
using System.Threading;
using NUnit.Framework;

namespace App.Testing.Infrastructure
{
    public class StartStopTimeProvider : FixedTimeProvider
    {
        private readonly ManualResetEventSlim _waitStart = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _waitEnd = new ManualResetEventSlim(false);

        public override void Wait(TimeSpan timeToWait)
        {
            base.Wait(timeToWait);

            _waitStart.Wait();
            _waitStart.Reset();
            _waitEnd.Set();
        }

        public void Continue()
        {
            _waitStart.Set();
        }

        public void WaitForIterationEnd()
        {
            Assert.That(_waitEnd.Wait(1000), Is.True);
            _waitEnd.Reset();
        }
    }
}
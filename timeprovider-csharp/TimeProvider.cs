namespace App.Common.Calendar
{
    public static class TimeProvider
    {
        public static ITimeProvider Current { get; private set; } = new SystemTimeProvider();

        public static void SetTimeProviderForTesting(ITimeProvider provider)
        {
            Current = provider;
        }

        public static void ResetProvider()
        {
            Current = new SystemTimeProvider();
        }
    }
}
namespace Common.Logging
{
    public static class LoggerContext
    {
        public static void SetThreadId(string id)
        {
            log4net.ThreadContext.Properties["ThreadId"] = id;
        }

        public static string GetThreadId()
        {
            return log4net.ThreadContext.Properties["ThreadId"] as string;
        }
    }
}
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace Brefix.Common.Logging
{
    public class WorkerThreadAppender : RollingFileAppender
    {
        private const string DefaultLayout =
            "%date [%thread] %-5level %logger [%ndc] - %message%newline";

        private readonly string _defaultDatePattern;
        private bool _filtersAdded;

        public string ThreadId { get; set; }

        public WorkerThreadAppender()
        {
            _defaultDatePattern = DatePattern;
            AppendToFile = true;
            RollingStyle = RollingMode.Date;
            MaxSizeRollBackups = 30;
            StaticLogFileName = false;
        }

        protected override bool FilterEvent(LoggingEvent loggingEvent)
        {
            if (!_filtersAdded)
            {
                AddFilter(new ThreadFilter { ThreadId = ThreadId });
                AddFilter(new DenyAllFilter());
                _filtersAdded = true;
            }

            return base.FilterEvent(loggingEvent);
        }

        public override void ActivateOptions()
        {
            File = File ?? @".\logs\";
            Layout = Layout ?? new PatternLayout(DefaultLayout);

            if (DatePattern == _defaultDatePattern)
                DatePattern = "yyyyMMdd'." + Name.Replace("Appender", "") + ".log'";

            base.ActivateOptions();
        }

        private class ThreadFilter : FilterSkeleton
        {
            public string ThreadId { private get; set; }

            public override FilterDecision Decide(LoggingEvent loggingEvent)
            {
                return LoggerContext.GetThreadId() == ThreadId
                    ? FilterDecision.Accept
                    : FilterDecision.Neutral;
            }
        }
    }
}
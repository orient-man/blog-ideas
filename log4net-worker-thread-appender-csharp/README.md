# log4net: 1 log file (appender) per thread

Usage:

    // when starting new thread
    LoggerContext.SetThreadId("unique name/key here");

In log4net config file:

    <appender name="MyWorkerThreadAppender" type="Common.Logging.WorkerThreadAppender, Common">
      <threadId value="MyWorkerThread" />
      <datePattern value="yyyyMMdd'.MyWorkerThread.log'"/>
      <!-- other filters must have acceptOnMatch=false -->
      <filter type="log4net.Filter.LevelRangeFilter">
        <acceptOnMatch value="false" />
        <levelMin value="INFO" />
        <levelMax value="FATAL" />
      </filter>
    </appender>
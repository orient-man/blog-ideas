using System;
using System.Collections.Generic;
using System.Threading;
using Common.Logging;
using FluentAssertions;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Common.Logging.Tests
{
    [TestFixture]
    public class WorkerThreadAppenderTests
    {
        private static readonly ILoggerRepository Repository = LogManager.CreateRepository("repo");

        [TearDown]
        public void TearDownEachTest()
        {
            LogManager.ResetConfiguration();
        }

        [Test]
        public void set_date_pattern()
        {
            // arrange
            var appender = new TestingWorkerThreadAppender { Name = "TestAppender" };

            // act
            appender.ActivateOptions();

            // assert
            appender.DatePattern.Should().Be("yyyyMMdd'.Test.log'");
        }

        [Test]
        public void does_not_log_event_when_thread_id_is_not_set()
        {
            // arrange
            var appender = new TestingWorkerThreadAppender { Name = "test", ThreadId = "test" };
            var log = CreateLogger(appender);

            // act
            RunThread(() => log.Error("should not be logged"));

            // assert
            appender.Events.Should().BeEmpty();
        }

        [Test]
        public void log_events_from_threads_with_matching_thread_id()
        {
            // arrange
            const string threadId = "test";
            var appender = new TestingWorkerThreadAppender { Name = "test", ThreadId = threadId };
            var log = CreateLogger(appender);

            // act
            RunThread(
                () =>
                {
                    LoggerContext.SetThreadId(threadId);
                    log.Error("should be logged");
                });

            // assert
            appender.Events.Should().NotBeEmpty();
        }

        private static void RunThread(Action action)
        {
            var thread = new Thread(() => action());
            thread.Start();
            thread.Join();
        }

        private static ILog CreateLogger(AppenderSkeleton appender)
        {
            var repo = (Hierarchy)Repository;
            appender.ActivateOptions();
            repo.Root.RemoveAllAppenders();
            repo.Root.AddAppender(appender);
            BasicConfigurator.Configure(repo);
            return LogManager.GetLogger("repo", "test");
        }

        private class TestingWorkerThreadAppender : WorkerThreadAppender
        {
            public readonly List<LoggingEvent> Events = new List<LoggingEvent>();

            public TestingWorkerThreadAppender()
            {
                LockingModel = new MinimalLock();
            }

            protected override void Append(LoggingEvent loggingEvent)
            {
                Events.Add(loggingEvent);
            }
        }
    }
}
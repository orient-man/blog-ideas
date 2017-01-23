using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Autofac.Wcf.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Autofac.Wcf.Extensions.Tests
{
    [TestFixture]
    public class AutomaticServiceProxyTests
    {
        private static string Log { get; set; }

        [SetUp]
        public void SetUpEachTest()
        {
            Log = "";
        }

        [Test]
        public void creates_proxy()
        {
            // arrange
            var foo = CreateFooServiceProxy<IFooService>();
            var text = "1-1 proxy using selected proxy caller";

            // act & assert
            foo.Return(13).Should().Be(13);
            foo.Print(text);
            Log.Should().Be(text);
        }

        [Test]
        public void creates_proxy_for_derrived_marker_interface()
        {
            // arrange
            var foo = CreateFooServiceProxy<IFooServiceMarker>();
            var text = "marker interface is useful when more than 1 proxy caller is need";

            // act & assert
            foo.Return(13).Should().Be(13);
            foo.Print(text);
            Log.Should().Be(text);
        }

        [Test]
        public void create_proxy_exposing_only_base_interface()
        {
            // arrange
            var foo = CreateFooServiceProxy<IPrintable>();
            var text = "it is possible to expose part of the contract using base interfaces";

            // act & assert
            foo.Print(text);
            Log.Should().Be(text);
        }

        [Test]
        public void unwraps_original_exception_from_target_invocation_exception_added_by_castle()
        {
            Assert.Throws<ArgumentException>(
                () => CreateFooServiceProxy<IFooService>().Print(null));
        }

        [Test]
        public void performance_test_without_proxy()
        {
            var foo = new FooService();
            Debug.WriteLine(MeasureTime(() => RepeatExecute(foo, 1000)));
        }

        [Test]
        public void performance_test_with_proxy()
        {
            var foo = CreateFooServiceProxy<IFooService>();
            // wolniej, ale dramatu nie ma... ok. 0.2ms per call (zależne od maszyny)
            Debug.WriteLine(MeasureTime(() => RepeatExecute(foo, 1000)));
        }

        private static void RepeatExecute(IFooService foo, int count)
        {
            Enumerable.Range(1, count).ToList().ForEach(i => foo.Return(i.ToString()));
        }

        private static long MeasureTime(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static TTarget CreateFooServiceProxy<TTarget>()
        {
            var builder = new ContainerBuilder();
            builder.RegisterWithProxy<TTarget, IFooService, FooProxyCaller>();
            var container = builder.Build();
            return container.Resolve<TTarget>();
        }

        private class FooProxyCaller : IServiceProxyCaller<IFooService>
        {
            private readonly IFooService _service;

            public FooProxyCaller()
            {
                _service = new FooService();
            }

            public void CallMethod(Expression<Action<IFooService>> action)
            {
                Debug.WriteLine("Calling method via proxy");
                action.Compile().Invoke(_service);
            }

            public TResult CallFunction<TResult>(Expression<Func<IFooService, TResult>> func)
            {
                Debug.WriteLine("Calling function via proxy");
                return func.Compile().Invoke(_service);
            }
        }

        public interface IFooServiceMarker : IFooService
        {
            // shoud be empty
        }

        public interface IFooService : IPrintable
        {
            T Return<T>(T value);
        }

        public interface IPrintable
        {
            void Print(string text);
        }

        private class FooService : IFooService
        {
            public void Print(string text)
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("Pusty tekst");

                System.Console.WriteLine(text);
                Log += text;
            }

            public T Return<T>(T value)
            {
                return value;
            }
        }
    }
}
using System;
using System.Linq.Expressions;

namespace Autofac.Wcf.Expressions
{
    // abstract away method/function call
    public interface IServiceProxyCaller<TService>
    {
        void CallMethod(Expression<Action<TService>> action);

        TResult CallFunction<TResult>(Expression<Func<TService, TResult>> func);
    }
}
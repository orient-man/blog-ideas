using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using Example.DataLayer.DataAccess;
using Example.Interfaces.DataAccess;
using Castle.DynamicProxy;

namespace Infrastructure
{
    public class InMemoryDatabase
    {
        private readonly IDictionary<string, Table> _db = new Dictionary<string, Table>();

        public T Insert<T>(T entity) where T : class
        {
            return CreateBasicDal<T>().Insert(entity);
        }

        // TODO: cache
        public IDataProvider CreateDataProvider()
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget(
                typeof(IDataProvider),
                new DataProviderMethodInterceptor(GetTable));
            return (IDataProvider)proxy;
        }

        // TODO: cache
        public IBasicDal<T> CreateBasicDal<T>() where T : class
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateInterfaceProxyWithoutTarget(
                typeof(IBasicDal<T>),
                new BasicDalMethodInterceptor(GetTable));
            return (IBasicDal<T>)proxy;
        }

        private class DataProviderMethodInterceptor : IInterceptor
        {
            private readonly Func<Type, Table> _getTable;

            public DataProviderMethodInterceptor(Func<Type, Table> getTable)
            {
                _getTable = getTable;
            }

            public void Intercept(IInvocation invocation)
            {
                if (!typeof(IQueryable).IsAssignableFrom(invocation.Method.ReturnType) ||
                    !invocation.Method.ReturnType.IsGenericType)
                    throw new NotSupportedException();

                var entityType = invocation.Method.ReturnType.GetGenericArguments().First();
                invocation.ReturnValue = _getTable(entityType).ToQueryable();
            }
        }

        private class BasicDalMethodInterceptor : IInterceptor
        {
            private readonly Func<Type, Table> _getTable;

            public BasicDalMethodInterceptor(Func<Type, Table> getTable)
            {
                _getTable = getTable;
            }

            public void Intercept(IInvocation invocation)
            {
                switch (invocation.Method.Name)
                {
                    case "Insert":
                        HandleInsert(invocation);
                        break;
                    case "PersistChanges":
                        HandlePersistChanges(invocation);
                        break;
                    case "PersistAllChanges":
                        HandlePersistAllChanges(invocation);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            private void HandleInsert(IInvocation invocation)
            {
                var table = _getTable(invocation.Method.ReturnType);
                invocation.ReturnValue = table.Insert(invocation.Arguments[0]);
            }

            private void HandlePersistChanges(IInvocation invocation)
            {
                var table = _getTable(invocation.Arguments[0].GetType());
                table.Update(invocation.Arguments[0]);
            }

            private void HandlePersistAllChanges(IInvocation invocation)
            {
                var entities = ((IEnumerable)invocation.Arguments[0]).Cast<object>().ToList();
                if (entities.Count == 0)
                    return;

                var table = _getTable(entities[0].GetType());
                foreach (var entity in entities)
                    table.Update(entity);
            }
        }

        private Table GetTable(Type entityType)
        {
            Table table;
            if (!_db.TryGetValue(entityType.Name, out table))
            {
                table = new Table(entityType);
                _db[entityType.Name] = table;
            }
            return table;
        }

        private class Table
        {
            private readonly PropertyInfo _primaryKey;
            private readonly IList _entities;
            private int _sequence;

            public Table(Type entityType)
            {
                var listType = typeof(List<>).MakeGenericType(entityType);
                _entities = (IList)Activator.CreateInstance(listType);
                _primaryKey = GetPrimaryKey(entityType);
            }

            private static PropertyInfo GetPrimaryKey(Type entityType)
            {
                return entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(IsPrimaryKey);
            }

            private static bool IsPrimaryKey(PropertyInfo propertyInfo)
            {
                return propertyInfo
                    .GetCustomAttributes<ColumnAttribute>()
                    .Any(p => p.IsPrimaryKey);
            }

            public object Insert(object entity)
            {
                var clone = CloneEntity(entity);
                if (_primaryKey != null)
                {
                    var key = (int)_primaryKey.GetValue(entity);
                    if (key <= 0)
                    {
                        key = ++_sequence;
                        _primaryKey.SetValue(entity, key);
                        _primaryKey.SetValue(clone, key);
                    }
                }
                _entities.Add(clone);
                return entity;
            }

            public void Update(object entity)
            {
                if (_primaryKey == null)
                    throw new NotSupportedException();

                var key = (int)_primaryKey.GetValue(entity);
                _entities.Remove(
                    _entities
                        .Cast<object>()
                        .First(o => (int)_primaryKey.GetValue(entity) == key));
                _entities.Add(CloneEntity(entity));
            }

            private static object CloneEntity(object entity)
            {
                return entity.GetType().GetMethod("Clone").Invoke(entity, new object[] { });
            }

            public IQueryable ToQueryable()
            {
                return _entities.AsQueryable();
            }
        }
    }
}
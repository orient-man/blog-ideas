using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace DataAccess
{
    public interface IDatabase : IDisposable
    {
        IDataReader ExecuteReader(string sql);
        void Execute(string sql, int commandTimeOut);
        void Execute(string sql);
    }

    public class Database : IDatabase
    {
        private IDbConnection _connection;

        public Database(Func<IDbConnection> connectionFactory)
        {
            _connection = connectionFactory();
            _connection.Open();
        }

        public IDataReader ExecuteReader(string sql)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = PrepareQuery(sql);
                return cmd.ExecuteReader();
            }
        }

        public void Execute(string sql)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = PrepareQuery(sql);
                cmd.ExecuteNonQuery();
            }
        }

        public void Execute(string sql, int commandTimeOut)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = PrepareQuery(sql);
                cmd.CommandTimeout = commandTimeOut;
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual string PrepareQuery(string sql)
        {
            return sql;
        }

        public void Dispose()
        {
            if (_connection == null) return;
            _connection.Dispose();
            _connection = null;
        }
    }

    private class TestingDatabase : Database
    {
        public TestingDatabase()
            : base(() => new SQLiteConnection("Data Source=:memory:;Version=3;New=True;"))
        {
        }

        protected override string PrepareQuery(string sql)
        {
            // for SQLServer
            return base.PrepareQuery(sql).Replace("[dbo].", "");
        }
    }
}

// Usage:
// First create schema:
// var db = new TestingDatabase();
// db.Execute(@"CREATE TABLE ...");
// ...then insert data
// db.Execute(@"INSERT INTO ...");
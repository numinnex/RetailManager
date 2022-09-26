using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RMDataManger.Library.Internal.DataAccess
{
    internal class SQLDataAccess : IDisposable
    {
        internal string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        internal List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        internal void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

            }
        }
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        internal void StartTransaction(string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            _connection = new SqlConnection(connectionString);
            _connection.Open();

            _transaction = _connection.BeginTransaction();

        }
        internal void SaveDataInTransaction<T>(string storedProcedure, T parameters)
        {
            _connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);

        }
        internal List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
        {

            List<T> rows = _connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction).ToList();

            return rows;
        }


        internal void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null; 
            _connection?.Close();
            _connection = null;
        }
        internal void RollBackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
            _connection?.Close();
            _connection = null;
        }

        public void Dispose()
        {
            CommitTransaction();
        }
    }
}

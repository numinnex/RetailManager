using System.Collections.Generic;
using System.Transactions;

namespace RMDataManger.Library.Internal.DataAccess
{
    public interface ISQLDataAccess
    {
        void Dispose();
        List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
        void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
        void SaveDataInTransaction<T>(string storedProcedure, T parameters);
        void StartTransaction(string connectionStringName);

        void CommitTransaction();
        void RollBackTransaction();
    }
}
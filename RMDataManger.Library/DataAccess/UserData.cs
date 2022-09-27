using Microsoft.Extensions.Configuration;
using RMDataManger.Library.Internal.DataAccess;
using RMDataManger.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManger.Library.DataAccess
{
    public class UserData
    {
        private IConfiguration _config;
        public UserData(IConfiguration config)
        {
            _config = config;

        }
        public List<UserModel> GetUserById(string Id)
        {
            SQLDataAccess sql = new SQLDataAccess(_config);

            var p = new { Id = Id }; 

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RMData");

            return output;
        }
    }
}

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
    public class UserData : IUserData
    {
        private ISQLDataAccess _sql;
        public UserData(ISQLDataAccess sql)
        {
            _sql = sql;
        }
        public List<UserModel> GetUserById(string Id)
        {

            var p = new { Id = Id };

            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "RMData");

            return output;
        }
        public void CreateUser(UserModel user)
        {
            _sql.SaveData("dbo.spUser_Insert", new { user.Id, user.FirstName, user.LastName, user.EmailAdress }, "RMData");
        }
    }
}

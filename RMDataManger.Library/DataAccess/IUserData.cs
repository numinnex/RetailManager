using RMDataManger.Library.Models;
using System.Collections.Generic;

namespace RMDataManger.Library.DataAccess
{
    public interface IUserData
    {
        void CreateUser(UserModel user);
        List<UserModel> GetUserById(string Id);
    }
}
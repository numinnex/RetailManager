﻿using RMDataManger.Library.Models;
using System.Collections.Generic;

namespace RMDataManger.Library.DataAccess
{
    public interface IUserData
    {
        List<UserModel> GetUserById(string Id);
    }
}
using RMDesktopUI.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.API
{
    public interface IUserEndPoint
    {
        Task<List<ApplicationUserModel>> GetAll();
    }
}
using RMDesktopUI.Library.Models;

namespace Portal.Registration
{
    public interface IRegister
    {
        Task RegisterUser(CreateUserModel user);
    }
}
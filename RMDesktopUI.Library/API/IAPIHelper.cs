using RMDesktopUI.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.API
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
        HttpClient ApiClient { get; }
    }
}
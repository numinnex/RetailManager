namespace Portal.Models
{
    public interface IAuthenticationService
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel user);
        Task Logout();
    }
}
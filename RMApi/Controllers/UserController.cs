using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RMApi.Data;
using RMApi.Models;
using RMDataManger.Library.DataAccess;
using RMDataManger.Library.Models;
using System.Security.Claims;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private ApplicationDbContext _context;
        private IUserData _data;
        private ILogger _logger;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager,  IUserData data, ILogger<UserController> logger)
        {
            _logger = logger;
            _context = context;
            _data = data;
            _userManager = userManager;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return _data.GetUserById(id).First();

        }

        public record UserRegistrationModel(string FirstName, string LastName, string EmailAdress, string Password);


    


        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]

        public async Task<IActionResult> Register(UserRegistrationModel user)   
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.EmailAdress);
                if (existingUser is null)
                {
                    IdentityUser newUser = new()
                    {
                        Email = user.EmailAdress,
                        EmailConfirmed = true,
                        UserName = user.EmailAdress
                    };
                    IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                    if (result.Succeeded)
                    {

                        existingUser = await _userManager.FindByEmailAsync(user.EmailAdress);

                        if (existingUser is null)
                        {
                            return BadRequest();
                        }

                        UserModel u = new()
                        {
                            Id = existingUser.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            EmailAdress = user.EmailAdress
                        };
                        _data.CreateUser(u);
                        return Ok();

                    }
                }
            }
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            var users = _context.Users.ToList();
            //var roles = _context.Roles.ToList();
            var userRoles = from ur in _context.UserRoles join r in _context.Roles on ur.RoleId equals r.Id select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(x => x.RoleId, x => x.Name);

                //foreach (var r in user.Roles)
                //{
                //    u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);

                //}
                output.Add(u);
            }
            return output;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {

            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pairing)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            _logger.LogInformation("Admin {Admin} added user {User} to role {Role}" , id , user.Id , pairing.Role );

            await _userManager.AddToRoleAsync(user, pairing.Role);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveFromRole")]
        public async Task RemoveFromRole(UserRolePairModel pairing)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(pairing.UserId);

            _logger.LogInformation("Admin {Admin} removed user {User} from role {Role}" , id , user.Id , pairing.Role );
            
            await _userManager.RemoveFromRoleAsync(user, pairing.Role);


        }
    }
}

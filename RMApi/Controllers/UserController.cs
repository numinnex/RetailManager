﻿using Microsoft.AspNetCore.Authorization;
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
        private IConfiguration _config;
        public UserController(IConfiguration config)
        {

            _config = config;
        }

        private readonly UserManager<IdentityUser> _userManager;

        private ApplicationDbContext _context;

        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public UserModel GetById()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserData data = new UserData(_config);

            return data.GetUserById(id).First();

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
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
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {

            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);
            return roles;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public async Task AddRole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.AddToRoleAsync(user, pairing.Role);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveFromRole")]
        public async Task RemoveFromRole(UserRolePairModel pairing)
        {

            var user = await _userManager.FindByIdAsync(pairing.UserId);
            _userManager.RemoveFromRoleAsync(user, pairing.Role);


        }
    }
}

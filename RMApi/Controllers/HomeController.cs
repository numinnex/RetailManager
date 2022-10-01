using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RMApi.Models;
using System.Diagnostics;

namespace RMApi.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            //string[] roles = { "Admin", "Manager", "Cashier" };

            //foreach (var role in roles)
            //{
            //    var roleExists = await _roleManager.RoleExistsAsync(role);

            //    if(!roleExists)
            //    {
            //        await _roleManager.CreateAsync(new IdentityRole(role));
            //    }

            //}

            //var user = await _userManager.FindByEmailAsync("admin@admin.com");

            //if(user != null)
            //{
            //    await _userManager.AddToRoleAsync(user, "Admin");
            //    await _userManager.AddToRoleAsync(user, "Cashier");
            //}

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
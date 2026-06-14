using ERP.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "User Management";
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Add User";
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string fullName, string email,
            string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction("Index");
            }

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            ViewData["Title"] = "Edit User";
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.CurrentRole = roles.FirstOrDefault();
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string fullName,
            string role, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = fullName;
            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, newPassword);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index");
        }
    }
}
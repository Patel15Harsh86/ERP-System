using ERP.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "My Profile";
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Update(string fullName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            user.FullName = fullName;
            await _userManager.UpdateAsync(user);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword,
            string newPassword, string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords do not match!";
                return RedirectToAction("Index");
            }

            var result = await _userManager.ChangePasswordAsync(
                user, currentPassword, newPassword);

            if (result.Succeeded)
                TempData["Success"] = "Password changed successfully!";
            else
                TempData["Error"] = string.Join(", ",
                    result.Errors.Select(e => e.Description));

            return RedirectToAction("Index");
        }
    }
}
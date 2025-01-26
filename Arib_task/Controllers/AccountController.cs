using Arib_task.DTOs;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Arib_task.Controllers;
public class AccountController(
        UserManager<AppUser> _userManager,
        SignInManager<AppUser> _signInManager)
    : Controller
{


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid input data." });
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return Json(new { success = false, message = "Invalid login attempt." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (result.Succeeded)
        {
            return Json(new { success = true, message = "Login successful!" });
        }

        return Json(new { success = false, message = "Invalid login attempt." });

    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.DisplayName,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerDto);
        }

        TempData["Success"] = "Registration successful! You can now log in.";
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public async Task<JsonResult> EmailExists(string email)
    {
        var exists = await _userManager.FindByEmailAsync(email) != null;
        return Json(exists);
    }

    // Logout
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Login");
    }

}
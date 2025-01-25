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
        SignInManager<AppUser> _signInManager,
        ITokenService _tokenService)
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
            return View(loginDto);
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            TempData["Error"] = "Invalid login attempt.";
            return View(loginDto);
        }

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!result.Succeeded)
        {
            TempData["Error"] = "Invalid login attempt.";
            return View(loginDto);
        }

        TempData["Success"] = "Login successful!";
        return RedirectToAction("Index", "Home"); 
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
        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await FindByEmailFromClaimsPrincipal(User);
        var userDto = new UserDTO
        {
            Email = user.Email,
            DisplayName = user.UserName,
            Token = _tokenService.GenerateToken(user)
        };

        return View(userDto); 
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

    private async Task<AppUser> FindByEmailFromClaimsPrincipal(ClaimsPrincipal user) =>
            await _userManager.Users.SingleOrDefaultAsync(x => x.Email == user.FindFirstValue(ClaimTypes.Email));
}
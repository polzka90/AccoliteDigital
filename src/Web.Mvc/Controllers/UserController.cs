using System.Security.Claims;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Mvc.Models;

namespace Web.Mvc.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [AllowAnonymous]
    // GET
    public IActionResult Index()
    {
        return Login();
    }
    
    [AllowAnonymous]
    // GET
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    // GET
    public async Task<IActionResult> Login(UserModel user)
    {
        var response = await _userService.LoginUserAsync(user.Email, user.Password);

        if (response is { Succeeded: true, Data: true })
        {
            var responseUser = await _userService.GetUserByEmailAsync(user.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, responseUser.Data.Email),
                new Claim("userId", responseUser.Data.Id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Account");
        }

        ModelState.AddModelError("Error",response.Message);
        
        
        return View();
    }
    
    [AllowAnonymous]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Login");
    }

    [AllowAnonymous]
    public ActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<ActionResult> Create(UserDto user)
    {
        await _userService.CreateNewUserAsync(user);
        
        return RedirectToAction(nameof(Login));
    }
}
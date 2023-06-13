using System.Security.Claims;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Mvc.Models;

namespace Web.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var user = HttpContext.User.Claims.FirstOrDefault(u => u.Type == "userId");
        int userId = Convert.ToInt32(user?.Value);
        
        var response = await _accountService.GetAllAccountByUserId(userId);
        
        return View(response.Data);
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountModel accountModel)
    {
        if (ModelState.IsValid)
        {
            var user = HttpContext.User.Claims.FirstOrDefault(u => u.Type == "userId");
            int id = Convert.ToInt32(user?.Value);
            
            var response = await _accountService.CreateNewAccountForUserAsync(id, accountModel.Amount);

            if (response.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("Error",response.Message);
        }
        return View();
    }
    public IActionResult Deposit(int id)
    {
        return View(new AccountModel(){Id = id});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(AccountModel accountModel)
    {
        var response = await _accountService.DepositMoney(
            accountModel.Id, accountModel.Amount);
        
        if (response.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("Error",response.Message);
        
        return View(new AccountModel(){Id = accountModel.Id});
    }
    public IActionResult Withdraw(int id)
    {
        return View(new AccountModel(){Id = id});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(AccountModel accountModel)
    {
        var response = await _accountService.WithdrawMoney(
            accountModel.Id, accountModel.Amount);
        if (response.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        ModelState.AddModelError("Error",response.Message);
        return View(new AccountModel(){Id = accountModel.Id});
    }
    
    public IActionResult Close(int id)
    {
        return View(new AccountModel(){Id = id});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Close(AccountModel accountModel)
    {
        var response = _accountService.CloseAccount(
            accountModel.Id);
        return RedirectToAction(nameof(Index));
    }
}
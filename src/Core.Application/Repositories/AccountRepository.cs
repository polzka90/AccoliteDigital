using Core.Application.Contracts.Interfaces;
using Core.Application.Entities;
using Core.Application.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Repositories;

internal sealed class AccountRepository : IAccountRepository
{
    private readonly BankContext _context;
 
    public AccountRepository(BankContext context)
    {
        _context = context;

    }

    public async Task<Account?> GetAccountById(int accountId)
    {
        return await _context.Accounts.FindAsync(accountId);
    }

    public async Task<Account?> AddAsync(Account account)
    {
       
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return await GetAccountById(account.Id);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
        _context.SaveChanges();
    }

    public void Delete(int accountId)
    {
        _context.Accounts.Remove(_context.Accounts.Find(accountId));
        _context.SaveChanges();
    }
}
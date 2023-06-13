using Core.Application.Contracts.Interfaces;
using Core.Application.Entities;
using Core.Application.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly BankContext _context;

    public UserRepository(BankContext context) 
    {        
        _context = context;
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User?> GetUserByEmail(string userEmail)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChangesAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return await GetUserById(user.Id);
    }
}
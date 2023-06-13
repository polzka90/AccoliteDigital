using Core.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Repositories.Context;

public sealed class BankContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    
    public BankContext(DbContextOptions<BankContext> options)
        : base(options)
    {
    }
}
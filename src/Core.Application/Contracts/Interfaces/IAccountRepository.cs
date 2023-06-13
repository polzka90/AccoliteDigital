using Core.Application.Entities;

namespace Core.Application.Contracts.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetAccountById(int accountId);
    Task<Account?> AddAsync(Account account);
    void Update(Account account);
    void Delete(int accountId);
}
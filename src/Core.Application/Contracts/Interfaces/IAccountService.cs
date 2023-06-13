using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Response;

namespace Core.Application.Contracts.Interfaces;

public interface IAccountService
{
    Task<Response<AccountDto>> CreateNewAccountForUserAsync(int userId, decimal amount);
    Task<Response<List<AccountDto>>> GetAllAccountByUserId(int userId);
    Task<Response<bool>> DepositMoney(int accountId, decimal amount);
    Task<Response<bool>> WithdrawMoney(int accountId, decimal amount);
    
    Response<bool> CloseAccount(int accountId);
}
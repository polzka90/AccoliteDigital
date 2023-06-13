using AutoMapper;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Interfaces;
using Core.Application.Contracts.Response;
using Core.Application.Entities;

namespace Core.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private const decimal MaxDepositAmountAllowed = 10_000;
    private const decimal MaxPercentageWithdrawAllowed = 90;
    private const decimal OneHundredPercentage = 100;
    private const decimal MinimumAmountAllowed = 100;
    
    public AccountService(IAccountRepository accountRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<Response<AccountDto>> CreateNewAccountForUserAsync(int userId, decimal amount)
    {
        User? user = await _userRepository.GetUserById(userId);

        if (user is null)
        {
            return Response<AccountDto>.Fail("Cannot Find the user");
        }
        
        if (amount < MinimumAmountAllowed)
        {
            return Response<AccountDto>.Fail($"The minimal amount to create an account is {MinimumAmountAllowed}");
        }
        
        var account = await _accountRepository.AddAsync(new Account()
        {
            Balance = amount,
            CreationDate = DateTime.Now,
            LastUpdatedDate = DateTime.Now
            
        });

        user.Accounts.Add(account);
        
        _userRepository.Update(user);
            
        return Response<AccountDto>
            .Success(_mapper.Map<AccountDto>(account), 
            $"Account Number:{account.Id} Created For User: {user.Email}");
    }

    public async Task<Response<List<AccountDto>>> GetAllAccountByUserId(int userId)
    {
        var user = await _userRepository.GetUserById(userId);
        return Response<List<AccountDto>>.Success(_mapper.Map<List<AccountDto>>(user.Accounts),String.Empty);
    }

    public async Task<Response<bool>> DepositMoney(int accountId, decimal amount)
    {
        if (amount > MaxDepositAmountAllowed)
        {
            return Response<bool>.Fail($"The deposit amount is out of the limit of {MaxDepositAmountAllowed}");
        }
        
        Account? account = await _accountRepository.GetAccountById(accountId);
        account.Balance += amount;
        account.LastUpdatedDate = DateTime.Now;
        _accountRepository.Update(account);
        
        return Response<bool>.Success(true, "Deposit made succesful");
    }

    public async Task<Response<bool>> WithdrawMoney(int accountId, decimal amount)
    {
        Account? account = await _accountRepository.GetAccountById(accountId);

        decimal balance = account.Balance;

        if (balance - amount < MinimumAmountAllowed)
        {
            return Response<bool>.Fail($"You cannot have less that {MinimumAmountAllowed} in your account");
        }

        if (WithdrawIsMoreThatMaxPercentageAllowed(amount, account.Balance))
        {
            return Response<bool>.Fail($"Your Withdraw request is above the limit: {MaxPercentageWithdrawAllowed}%");
        }
        
        account.Balance -= amount;
        account.LastUpdatedDate = DateTime.Now;
        _accountRepository.Update(account);
        return Response<bool>.Success(true, $"You have succesful withdraw {amount}");
    }

    public Response<bool> CloseAccount(int accountId)
    {
        _accountRepository.Delete(accountId);
        
        return Response<bool>.Success(true, $"Account successful Delete");
    }

    private bool WithdrawIsMoreThatMaxPercentageAllowed(decimal amount, decimal balance)
    {
        return ((amount * OneHundredPercentage) / balance) > MaxPercentageWithdrawAllowed;
    }
}
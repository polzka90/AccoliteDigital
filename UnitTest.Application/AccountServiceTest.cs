using AutoMapper;
using Core.Application.Contracts.AutomapperProfiles;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Interfaces;
using Core.Application.Contracts.Response;
using Core.Application.Entities;
using Core.Application.Services;
using Moq;
using UnitTest.Application.Fakers;

namespace UnitTest.Application;

public class AccountServiceTest
{
    private IAccountService _accountService;
    private readonly Mock<IAccountRepository> _accountRepository = new Mock<IAccountRepository>();
    private readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    private IMapper _mapper;
    
    [SetUp]
    public void Setup()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserServiceProfile());
            mc.AddProfile(new AccoutServiceProfile());
        });
        _mapper = mappingConfig.CreateMapper();
    }
    
    [TestCase(1,100)]
    [TestCase(2,300)]
    public async Task NewAccount_ShouldBe_AddSuccessful(int userId, decimal amount)
    {
        //Arrange
        UserDto userToBeCreated = FakerUserGenerator.SingleUserDtoFake();

        Account account = new Account()
        {
            Id = 1,
            Balance = amount
        };
        
        User userEntity = _mapper.Map<User>(userToBeCreated);

        userEntity.Id = userId;
        
        _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(userEntity);

        _accountRepository.Setup(a => a.AddAsync(It.IsAny<Account>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<AccountDto> response = await _accountService.CreateNewAccountForUserAsync(userEntity.Id, amount);
        
        
        //Assert
        Assert.True(response.Succeeded);
        Assert.Greater(response.Data.Id,0);

        Assert.That(response.Data.Id, Is.EqualTo(account.Id));
        Assert.That(response.Data.Balance, Is.EqualTo(account.Balance));
    }
    
    [TestCase(1,100)]
    [TestCase(2,300)]
    public async Task NewAccount_ShouldNotBe_Add_When_UserDoNotExist(int userId, decimal amount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = 1,
            Balance = amount
        };

        _accountRepository.Setup(a => a.AddAsync(It.IsAny<Account>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<AccountDto> response = await _accountService.CreateNewAccountForUserAsync(userId, amount);
        
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.That(response.Message, Is.EqualTo("Cannot Find the user"));
    }
    
    [TestCase(1,90)]
    [TestCase(2,80)]
    public async Task NewAccount_ShouldNotBe_Add_When_AmountIsLessThat100(int userId, decimal amount)
    {
        //Arrange
        UserDto userToBeCreated = FakerUserGenerator.SingleUserDtoFake();


        
        User userEntity = _mapper.Map<User>(userToBeCreated);

        userEntity.Id = userId;
        
        _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(userEntity);
        
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<AccountDto> response = await _accountService.CreateNewAccountForUserAsync(userEntity.Id, amount);
        
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.That(response.Message, Is.EqualTo("The minimal amount to create an account is 100"));
    }

    [Test]
    public void Account_ShouldBe_Delete()
    {
        //Arrange
        int accountId = 1;
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = _accountService.CloseAccount(accountId);
        
        
        //Assert
        Assert.True(response.Succeeded);
        Assert.True(response.Data);
        Assert.That(response.Message, Is.EqualTo("Account successful Delete"));
    }
    
    [TestCase(1,100, 200)]
    [TestCase(2,300,150)]
    public async Task Account_Should_Deposit_Successful(int accountId, decimal balance ,decimal depositAmount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = accountId,
            Balance = balance
        };

        _accountRepository.Setup(a => a.GetAccountById(It.IsAny<int>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = await _accountService.DepositMoney(account.Id, depositAmount);
        
        
        //Assert
        Assert.True(response.Succeeded);
        Assert.True(response.Data);

        Assert.That(account.Balance, Is.EqualTo(balance + depositAmount));
    }
    
    [TestCase(1,100, 20000)]
    [TestCase(2,300,10001)]
    public async Task Account_ShouldNot_Deposit_When_AmountIsMoreThat10000(int accountId, decimal balance ,decimal depositAmount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = accountId,
            Balance = balance
        };

        _accountRepository.Setup(a => a.GetAccountById(It.IsAny<int>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = await _accountService.DepositMoney(account.Id, depositAmount);
        
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.False(response.Data);
        Assert.That(response.Message, Is.EqualTo("The deposit amount is out of the limit of 10000"));

        Assert.That(account.Balance, Is.EqualTo(balance));
    }
    
    [TestCase(1,1000, 200)]
    [TestCase(2,3000,150)]
    public async Task Account_Should_Withdraw_Successful(int accountId, decimal balance ,decimal withdrawAmount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = accountId,
            Balance = balance
        };

        _accountRepository.Setup(a => a.GetAccountById(It.IsAny<int>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = await _accountService.WithdrawMoney(account.Id, withdrawAmount);
        
        
        //Assert
        Assert.True(response.Succeeded);
        Assert.True(response.Data);

        Assert.That(account.Balance, Is.EqualTo(balance - withdrawAmount));
    }

    [TestCase(1,100, 91)]
    [TestCase(2,300,288)]
    public async Task Account_ShouldNot_Withdraw_When_BalanceWillBeLessThat100(int accountId, decimal balance ,decimal withdrawAmount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = accountId,
            Balance = balance
        };

        _accountRepository.Setup(a => a.GetAccountById(It.IsAny<int>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = await _accountService.WithdrawMoney(account.Id, withdrawAmount);
        
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.False(response.Data);
        Assert.That(response.Message, Is.EqualTo("You cannot have less that 100 in your account"));

        Assert.That(account.Balance, Is.EqualTo(balance));
    }
    
    [TestCase(1,10000, 9001)]
    [TestCase(2,3000,2880)]
    public async Task Account_ShouldNot_Withdraw_When_AmountIsMoreThat90Percentage(int accountId, decimal balance ,decimal withdrawAmount)
    {
        //Arrange
        Account account = new Account()
        {
            Id = accountId,
            Balance = balance
        };

        _accountRepository.Setup(a => a.GetAccountById(It.IsAny<int>()))
            .ReturnsAsync(account);
        
        _accountService = new AccountService(_accountRepository.Object, _userRepository.Object,_mapper);
        
        //Act
        Response<bool> response = await _accountService.WithdrawMoney(account.Id, withdrawAmount);
        
        
        //Assert
        Assert.False(response.Succeeded);
        Assert.False(response.Data);
        Assert.That(response.Message, Is.EqualTo("Your Withdraw request is above the limit: 90%"));

        Assert.That(account.Balance, Is.EqualTo(balance));
    }
}
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

public class UserServiceTest
{
    private IUserService _userService;
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

    [Test]
    public async Task UserMustBeCreatedSuccessful()
    {
        //Arrange
        UserDto userToBeCreated = FakerUserGenerator.SingleUserDtoFake();
        User userEntity = _mapper.Map<User>(userToBeCreated);

        userEntity.Id = 1;

        _userRepository.Setup(ur => ur.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(userEntity);
        _userRepository.Setup(ur => ur.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(userEntity);
        
        _userService = new UserService(_userRepository.Object,_mapper);
        
        //Act
        Response<int> response = await _userService.CreateNewUserAsync(userToBeCreated);
        
        
        //Assert
        Assert.True(response.Succeeded);
        Assert.Greater(response.Data,0);

        Response<UserDto> userResponse = await _userService.GetUserByIdAsync(response.Data);

        Assert.That(userToBeCreated.Email, Is.EqualTo(userResponse.Data.Email));
    }
}
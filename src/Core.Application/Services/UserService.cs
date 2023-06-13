using AutoMapper;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Interfaces;
using Core.Application.Contracts.Response;
using Core.Application.Entities;

namespace Core.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public UserService(IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<Response<UserDto>> GetUserByIdAsync(int userId)
    {
        User? user = await _userRepository.GetUserById(userId);

        return Response<UserDto>
            .Success(_mapper.Map<UserDto>(user), String.Empty);
    }

    public async Task<Response<UserDto>> GetUserByEmailAsync(String email)
    {
        User? user = await _userRepository.GetUserByEmail(email);

        return Response<UserDto>
            .Success(_mapper.Map<UserDto>(user), String.Empty);
    }

    public async Task<Response<bool>> LoginUserAsync(string email, string password)
    {
        User? user = await _userRepository.GetUserByEmail(email);

        if (user is null || user.Password != password)
        {
            return Response<bool>.Success(false, "Wrong User or Password");
        }
        
        return Response<bool>.Success(true, "User and Password Validated");
    }

    public async Task<Response<int>> CreateNewUserAsync(UserDto userDto)
    {
        User user = _mapper.Map<User>(userDto);
        
        user.CreationDate = DateTime.Now;
        user.LastUpdatedDate = DateTime.Now;
        
        User userEntity = await _userRepository.AddAsync(user);
        
        return Response<int>.Success(userEntity.Id, "User Created");
    }
}
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Contracts.Response;

namespace Core.Application.Contracts.Interfaces;

public interface IUserService
{
    Task<Response<UserDto>> GetUserByIdAsync(int userId);
    Task<Response<UserDto>> GetUserByEmailAsync(string email);
    Task<Response<bool>> LoginUserAsync(string email, string password);
    Task<Response<int>> CreateNewUserAsync(UserDto user);
}
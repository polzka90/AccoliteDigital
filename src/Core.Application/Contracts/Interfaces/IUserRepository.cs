using Core.Application.Entities;

namespace Core.Application.Contracts.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> GetUserById(int userId);
    Task<User?> GetUserByEmail(string userEmail);
    void Update(User user);
}
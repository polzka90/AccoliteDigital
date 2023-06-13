using AutoMapper;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Entities;

namespace Core.Application.Contracts.AutomapperProfiles;

public sealed class UserServiceProfile : Profile
{
    public UserServiceProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
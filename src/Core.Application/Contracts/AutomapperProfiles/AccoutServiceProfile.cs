using AutoMapper;
using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Entities;

namespace Core.Application.Contracts.AutomapperProfiles;

public sealed class AccoutServiceProfile : Profile
{
    public AccoutServiceProfile()
    {
        CreateMap<Account, AccountDto>().ReverseMap();
    }
}
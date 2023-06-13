using Core.Application.Contracts.DataTransferObjects;
using Core.Application.Entities;

namespace Web.Mvc.Models;

public class UserModel : UserDto
{
    public string? Error { get; set; }
}
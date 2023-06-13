using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Core.Application.Entities;

public class User : BaseEntity
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }
    public List<Account?> Accounts { get; set; }
}
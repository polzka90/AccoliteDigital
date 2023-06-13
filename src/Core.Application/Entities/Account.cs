using System.ComponentModel.DataAnnotations;

namespace Core.Application.Entities;

public class Account : BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    public decimal Balance { get; set; }
}
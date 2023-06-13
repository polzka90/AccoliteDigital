namespace Core.Application.Contracts.DataTransferObjects;

public sealed class AccountDto
{
    public int Id { get; set; }
    
    public decimal Balance { get; set; }
}
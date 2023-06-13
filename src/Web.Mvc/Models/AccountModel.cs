using System.Runtime.InteropServices.JavaScript;

namespace Web.Mvc.Models;

public class AccountModel
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string? Error { get; set; }
}
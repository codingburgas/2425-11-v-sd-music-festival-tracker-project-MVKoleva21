using System.ComponentModel.DataAnnotations.Schema;
namespace Ticketer.DAL.Models;

[Table("[dbo].[User]")]

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public decimal Deposit { get; set; }
}
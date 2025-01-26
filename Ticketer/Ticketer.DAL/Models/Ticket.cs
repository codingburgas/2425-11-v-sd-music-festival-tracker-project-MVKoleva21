using System.ComponentModel.DataAnnotations.Schema;
namespace Ticketer.DAL.Models;

[Table("[dbo].[Tickets]")]

public class Ticket
{
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public int ConcertId { get; set; }
    public DateTime PurchaseDate { get; set; }

    public User User { get; set; }
    public Concert Concert { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
namespace Ticketer.DAL.Models;

[Table("[dbo].[Concerts]")]

public class Concert
{
    public int ConcertId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
}
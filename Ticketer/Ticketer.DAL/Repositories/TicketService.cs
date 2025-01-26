using Ticketer.DAL.Data;
using Ticketer.DAL.Models;

namespace Ticketer.DAL.TicketService
{
    public class TicketService
    {
        private readonly TicketManagementDatabaseContext _dbContext;

        public TicketService(TicketManagementDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool PurchaseTicket(int userId, int concertId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            var concert = _dbContext.Concerts.FirstOrDefault(c => c.ConcertId == concertId);

            if (user == null || concert == null || user.Deposit < concert.Price) return false;

            user.Deposit -= concert.Price;
            _dbContext.Tickets.Add(new Ticket
            {
                UserId = userId,
                ConcertId = concertId,
                PurchaseDate = DateTime.Now
            });

            return true; // Implement saving to the database
        }

        public async Task<List<Ticket>> GetUserTicketsAsync(int userId)
        {
            return await Task.Run(() =>
                _dbContext.Tickets
                .Where(t => t.UserId == userId)
                .ToList()
            );
        }

        public async Task<bool> CreateTicketAsync(Ticket ticket)
        {
            if (ticket == null) return false;

            try
            {
                _dbContext.Tickets.Add(ticket);

                await Task.Run(() => _dbContext.Connection.CreateCommand().ExecuteNonQuery());

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

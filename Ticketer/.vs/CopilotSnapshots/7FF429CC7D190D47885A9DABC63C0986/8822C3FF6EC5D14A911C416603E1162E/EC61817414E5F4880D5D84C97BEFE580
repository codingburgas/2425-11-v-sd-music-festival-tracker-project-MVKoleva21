using Ticketer.DAL.Data;
using Ticketer.DAL.Models;

namespace Ticketer.DAL.UserService
{
    public class UserService
    {
        private readonly TicketManagementDatabaseContext _dbContext;

        public UserService(TicketManagementDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public void AddDeposit(int userId, decimal amount)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.Deposit += amount;
                // Save changes to database (implement as needed)
            }
        }
    }
}

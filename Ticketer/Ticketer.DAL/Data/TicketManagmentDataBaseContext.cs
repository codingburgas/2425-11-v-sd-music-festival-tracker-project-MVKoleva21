using Ticketer.DAL.Models;
using Microsoft.Data.SqlClient;

namespace Ticketer.DAL.Data
{
    public class TicketManagementDatabaseContext
    {
        public SqlConnection Connection { get; private set; }

        public List<User> Users { get; set; } = new List<User>();
        public List<Concert> Concerts { get; set; } = new List<Concert>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();

        public TicketManagementDatabaseContext()
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=TicketerDB;Trusted_Connection=True;";
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            ReadUsers();
            ReadConcerts();
            ReadTickets();
        }

        public void ReadUsers()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Users]", Connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Users.Add(new User()
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Email = Convert.ToString(reader["Email"]),
                    PasswordHash = Convert.ToString(reader["PasswordHash"]),
                    Role = Convert.ToString(reader["Role"]),
                    Deposit = Convert.ToDecimal(reader["Deposit"])
                });
            }

            reader.Close();
        }

        public void ReadConcerts()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Concerts]", Connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Concerts.Add(new Concert()
                {
                    ConcertId = Convert.ToInt32(reader["ConcertId"]),
                    Title = Convert.ToString(reader["Title"]),
                    Description = Convert.ToString(reader["Description"]),
                    Date = Convert.ToDateTime(reader["Date"]),
                    Price = Convert.ToDecimal(reader["Price"])
                });
            }

            reader.Close();
        }

        public void ReadTickets()
        {
            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Tickets]", Connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Tickets.Add(new Ticket()
                {
                    TicketId = Convert.ToInt32(reader["TicketId"]),
                    PurchaseDate = Convert.ToDateTime(reader["PurchaseDate"]),
                    UserId = Convert.ToInt32(reader["UserId"]),
                    ConcertId = Convert.ToInt32(reader["ConcertId"])
                });
            }

            reader.Close();
        }
    }
}

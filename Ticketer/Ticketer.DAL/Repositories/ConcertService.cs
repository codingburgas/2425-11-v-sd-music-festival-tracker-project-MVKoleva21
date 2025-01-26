using System.Data;
using Microsoft.Data.SqlClient;
using Ticketer.DAL.Data;
using Ticketer.DAL.Models;

namespace Ticketer.DAL.ConcertService
{
    public class ConcertService
    {
        private readonly TicketManagementDatabaseContext _dbContext;

        public ConcertService(TicketManagementDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddConcertAsync(Concert concert)
        {
            const string query = "INSERT INTO [dbo].[Concerts] (Title, Description, Date, Price) VALUES (@Title, @Description, @Date, @Price)";
            using (var command = new SqlCommand(query, _dbContext.Connection))
            {
                command.Parameters.AddWithValue("@Title", concert.Title);
                command.Parameters.AddWithValue("@Description", concert.Description);
                command.Parameters.AddWithValue("@Date", concert.Date);
                command.Parameters.AddWithValue("@Price", concert.Price);

                await EnsureConnectionOpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Concert> GetConcertByIdAsync(int concertId)
        {
            const string query = "SELECT * FROM [dbo].[Concerts] WHERE ConcertId = @ConcertId";
            using (var command = new SqlCommand(query, _dbContext.Connection))
            {
                command.Parameters.AddWithValue("@ConcertId", concertId);

                await EnsureConnectionOpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        return new Concert
                        {
                            ConcertId = reader.GetInt32(reader.GetOrdinal("ConcertId")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                        };
                    }
                }
            }
            return null;
        }

        public async Task UpdateConcertAsync(Concert concert)
        {
            const string query = "UPDATE [dbo].[Concerts] SET Title = @Title, Description = @Description, Date = @Date, Price = @Price WHERE ConcertId = @ConcertId";
            using (var command = new SqlCommand(query, _dbContext.Connection))
            {
                command.Parameters.AddWithValue("@Title", concert.Title);
                command.Parameters.AddWithValue("@Description", concert.Description);
                command.Parameters.AddWithValue("@Date", concert.Date);
                command.Parameters.AddWithValue("@Price", concert.Price);
                command.Parameters.AddWithValue("@ConcertId", concert.ConcertId);

                await EnsureConnectionOpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteConcertAsync(int concertId)
        {
            const string query = "DELETE FROM [dbo].[Concerts] WHERE ConcertId = @ConcertId";
            using (var command = new SqlCommand(query, _dbContext.Connection))
            {
                command.Parameters.AddWithValue("@ConcertId", concertId);

                await EnsureConnectionOpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task EnsureConnectionOpenAsync()
        {
            if (_dbContext.Connection.State == ConnectionState.Closed)
            {
                await _dbContext.Connection.OpenAsync();
            }
        }

        public async Task<List<Concert>> GetAllConcertsAsync()
        {
            const string query = "SELECT * FROM [dbo].[Concerts]";
            var concerts = new List<Concert>();

            using (var command = new SqlCommand(query, _dbContext.Connection))
            {
                await EnsureConnectionOpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        concerts.Add(new Concert
                        {
                            ConcertId = reader.GetInt32(reader.GetOrdinal("ConcertId")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"))
                        });
                    }
                }
            }

            return concerts;
        }

    }
}

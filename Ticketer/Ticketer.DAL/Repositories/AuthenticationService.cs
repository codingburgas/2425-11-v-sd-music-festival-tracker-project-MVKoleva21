using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Ticketer.DAL.Models;
using Ticketer.DAL.Data;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Ticketer.DAL.AuthenticationService
{
    public class AuthenticationService
    {
        private readonly TicketManagementDatabaseContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(TicketManagementDatabaseContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<AuthenticationService> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> RegisterUser(string email, string password, string role = "User")
        {
            try
            {
                // Check if the user already exists
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == email);
                if (existingUser != null)
                {
                    _logger.LogWarning("User with email {Email} already exists.", email);
                    return false;
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Create new user
                var user = new User
                {
                    Email = email,
                    PasswordHash = passwordHash,
                    Role = role,
                    Deposit = 0  // Default deposit amount
                };

                // Insert the user into the database
                SqlCommand command = new SqlCommand("INSERT INTO [User] (Email, PasswordHash, Role, Deposit) VALUES (@Email, @PasswordHash, @Role, @Deposit)", _dbContext.Connection);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Deposit", user.Deposit);

                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("User with email {Email} registered successfully.", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email {Email}.", email);
                return false;
            }
        }

        public async Task<bool> LoginUser(string email, string password)
        {
            try
            {
                // Fetch user by email
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}.", email);
                    return false;
                }

                // Create the claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign the user in
                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                _logger.LogInformation("User with email {Email} logged in successfully.", email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user with email {Email}.", email);
                return false;
            }
        }

        public async Task LogoutUser()
        {
            try
            {
                // Log the user out by clearing the authentication cookie
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _logger.LogInformation("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out user.");
            }
        }

        public async Task UpdateUserDepositAsync(User user)
        {
            try
            {
                var command = new SqlCommand("UPDATE [User] SET Deposit = @Deposit WHERE UserId = @UserId", _dbContext.Connection);
                command.Parameters.AddWithValue("@Deposit", user.Deposit);
                command.Parameters.AddWithValue("@UserId", user.UserId);

                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("User deposit updated successfully for userId {UserId}.", user.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating deposit for userId {UserId}.", user.UserId);
            }
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            try
            {
                var userEmail = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail)) return null;

                return await Task.Run(() =>
                    _dbContext.Users.FirstOrDefault(u => u.Email == userEmail)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching authenticated user.");
                return null;
            }
        }

        public User CurrentUser
        {
            get
            {
                try
                {
                    // Assuming the user is stored in the HttpContext
                    var userEmail = _httpContextAccessor.HttpContext.User.Identity.Name;
                    return _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching current user.");
                    return null;
                }
            }
        }
    }
}

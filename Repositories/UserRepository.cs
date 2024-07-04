using System.Data;
using Dapper;
using System.Threading.Tasks;
using SecondRoundProject.Models;

namespace SecondRoundProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnection dbConnection, ILogger<UserRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        /// <summary>
        /// Checks if any Application User already exists with username.
        /// </summary>
        /// <param name="username">The username of user to search.</param>
        /// <returns>Return the matched Application User object.</returns>
        public async Task<ApplicationUser?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationUser>(
                "SELECT Id, Username, PasswordHash, FirstName, LastName, Role FROM ApplicationUser WHERE Username = @Username", new { Username = username });
                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", username);
                    return null;
                }

                _logger.LogInformation("User found: {Username}", username);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user: {Username}", username);
                throw;
            }
        }

        /// <summary>
        /// Adds a new Application User to the database.
        /// </summary>
        /// <param name="user">The application user object to be added.</param>
        /// <returns>Return the id of newly inserted application user.</returns>
        public async Task<int?> CreateUserAsync(ApplicationUser user)
        {
            try
            {
                var query = @"INSERT INTO ApplicationUser (Username, PasswordHash, FirstName, LastName, Role)
                          VALUES (@Username, @PasswordHash, @FirstName, @LastName, @Role);
                          SELECT CAST(SCOPE_IDENTITY() as int)";
                var result = await _dbConnection.QuerySingleAsync<int>(query, user);

                _logger.LogInformation("User created successfully: {Username}", user.Username);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating user: {Username}", user.Username);
                throw;
            }
        }
    }
}

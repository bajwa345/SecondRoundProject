using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SecondRoundProject.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SecondRoundProject.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IDistributedCache _cache;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IDbConnection dbConnection, IDistributedCache cache, ILogger<ClientRepository> logger)
        {
            _dbConnection = dbConnection;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of clients with filtering and sorting options.
        /// </summary>
        /// <param name="filter">The filter string to search for clients by personal id, first name or last name.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of clients per page.</param>
        /// <param name="sortBy">The column name to sort by.</param>
        /// <param name="sortOrder">The sort order for the selected column.</param>
        /// <returns>A tuple containing the list of clients and the total count of clients matching the filter.</returns>
        public async Task<(IEnumerable<Client> Clients, int TotalCount)> GetClientsAsync(string? filter, int page, int pageSize, string sortBy = "Id", string sortOrder = "ASC")
        {
            try
            {
                var query = $@"
                SELECT COUNT(*) FROM Client WHERE (@Filter IS NULL OR PersonalId LIKE @Filter OR FirstName LIKE @Filter OR LastName LIKE @Filter);
                SELECT * FROM Client WHERE (@Filter IS NULL OR PersonalId LIKE @Filter OR FirstName LIKE @Filter OR LastName LIKE @Filter)
                ORDER BY {sortBy} {sortOrder} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                using (var multi = await _dbConnection.QueryMultipleAsync(query, new
                {
                    Filter = string.IsNullOrEmpty(filter) ? null : $"%{filter}%",
                    Offset = (page - 1) * pageSize,
                    PageSize = pageSize
                }))
                {
                    var totalCount = multi.Read<int>().Single();
                    var clients = multi.Read<Client>().ToList();

                    //get all accounts for client
                    foreach (var client in clients)
                    {
                        var accountsQuery = "SELECT * FROM ClientAccount WHERE ClientId = @ClientId";
                        client.Accounts = (await _dbConnection.QueryAsync<ClientAccount>(accountsQuery, new { ClientId = client.Id })).ToList();
                    }
                    return (clients, totalCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving clients.");
                throw;
            }
        }

        /// <summary>
        /// Adds a new client to the database.
        /// </summary>
        /// <param name="client">The client object to be added.</param>
        public async Task AddClientAsync(Client client)
        {
            if (_dbConnection.State == ConnectionState.Closed) _dbConnection.Open();
            
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    var clientQuery = @"
                INSERT INTO Client (PersonalId, Email, FirstName, LastName, ProfilePhoto, MobileNumber, Sex, Country, City, Street, ZipCode, CreatedBy, CreatedAt)
                VALUES (@PersonalId, @Email, @FirstName, @LastName, @ProfilePhoto, @MobileNumber, @Sex, @Country, @City, @Street, @ZipCode, @CreatedBy, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";

                    var clientId = await _dbConnection.QuerySingleAsync<int>(clientQuery, new
                    {
                        client.PersonalId,
                        client.Email,
                        client.FirstName,
                        client.LastName,
                        client.ProfilePhoto,
                        client.MobileNumber,
                        client.Sex,
                        client.Country,
                        client.City,
                        client.Street,
                        client.ZipCode,
                        client.CreatedBy
                    }, transaction);

                    //insert all accounts for the client in database
                    if (client.Accounts != null && client.Accounts.Any())
                    {
                        var accountQuery = @"
                        INSERT INTO ClientAccount (ClientId, AccountNumber)
                        VALUES (@ClientId, @AccountNumber)";

                        foreach (var account in client.Accounts)
                        {
                            await _dbConnection.ExecuteAsync(accountQuery, new
                            {
                                ClientId = clientId,
                                AccountNumber = account.AccountNumber
                            }, transaction);
                        }
                    }

                    transaction.Commit();
                    _logger.LogInformation("Client and accounts added successfully: {PersonalId}", client.PersonalId);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, "An error occurred while adding a client and their accounts: {PersonalId}", client.PersonalId);
                    throw;
                }
            }
        }

        //// <summary>
        /// Retrieves the last three search parameters from the cache.
        /// </summary>
        /// <returns>A list of the last three search parameters.</returns>
        public List<string>? GetLastSearchParameters()
        {
            try
            {
                string? cachedParameters = _cache.GetString("lastSearchParameters");
                if (!string.IsNullOrEmpty(cachedParameters))
                {
                    return JsonConvert.DeserializeObject<List<string>>(cachedParameters);
                }

                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the last search parameters from the cache.");
                throw;
            }
        }

        /// <summary>
        /// Adds a new search parameter to the cache, maintaining only the last three search parameters.
        /// </summary>
        /// <param name="searchParameter">The search parameter to add.</param>
        public void AddSearchParameter(string searchParameter)
        {
            try
            {
                var cachedParameters = GetLastSearchParameters();
                if (cachedParameters != null && cachedParameters.Contains(searchParameter))
                {
                    cachedParameters.Remove(searchParameter);
                }

                if (cachedParameters == null) cachedParameters = new List<string>();
                cachedParameters.Insert(0, searchParameter);
                if (cachedParameters.Count > 3)
                {
                    cachedParameters = cachedParameters.Take(3).ToList();
                }

                _cache.SetString("lastSearchParameters", JsonConvert.SerializeObject(cachedParameters));

                _logger.LogInformation("Search parameter added: {SearchParameter}", searchParameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a search parameter to the cache.");
                throw;
            }
        }
    }
}
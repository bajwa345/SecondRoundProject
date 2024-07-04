using System.Collections.Generic;
using System.Threading.Tasks;
using SecondRoundProject.Models;

namespace SecondRoundProject.Repositories
{
    public interface IClientRepository
    {
        Task<(IEnumerable<Client> Clients, int TotalCount)> GetClientsAsync(string? filter, int page, int pageSize, string sortBy, string sortOrder);
        Task AddClientAsync(Client client);
        List<string>? GetLastSearchParameters();
        void AddSearchParameter(string searchParameter);
    }
}

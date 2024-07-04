using System.Collections.Generic;
using System.Threading.Tasks;
using SecondRoundProject.DTOs;

namespace SecondRoundProject.Services
{
    public interface IClientService
    {
        Task<(IEnumerable<ClientDTO> Items, int TotalCount)> GetClientsAsync(string? filter, int page, int pageSize, string? sortBy, string? sortOrder);
        Task AddClientAsync(ClientDTO clientDTO);
        List<string> GetLastSearchParameters();
    }
}

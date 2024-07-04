using AutoMapper;
using SecondRoundProject.DTOs;
using SecondRoundProject.Helpers;
using SecondRoundProject.Models;
using SecondRoundProject.Repositories;

namespace SecondRoundProject.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IClientRepository clientRepository, IMapper mapper, ILogger<ClientService> logger)
        {
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<ClientDTO> Items, int TotalCount)> GetClientsAsync(string? filter, int page, int pageSize, string? sortBy, string? sortOrder)
        {
            _logger.LogInformation("Fetching clients with filter: {Filter}, page: {Page}, pageSize: {PageSize}", filter, page, pageSize);

            //check if sort order is valid, else set it as 'ASC'
            var validSortColumns = ReflectionHelper.GetPropertyNames<Client>();
            if (!validSortColumns.Contains(sortBy ?? "Id")) sortBy = "Id";

            //check if sort order is valid, else set it as 'ASC'
            var validSortOrders = new List<string> { "ASC", "DESC" };
            if (!validSortOrders.Contains(sortOrder ?? "ASC")) sortOrder = "ASC";

            var (clients, totalCount) = await _clientRepository.GetClientsAsync(filter, page, pageSize, sortBy??"Id", sortOrder??"ASC");
            
            //save last three search parameters in cache
            if(!string.IsNullOrEmpty(filter)) _clientRepository.AddSearchParameter(filter);
            var clientDTOs = _mapper.Map<IEnumerable<ClientDTO>>(clients);

            _logger.LogInformation("Retrieved {Count} clients", clientDTOs.Count());

            return (clientDTOs, totalCount);
        }

        public async Task AddClientAsync(ClientDTO clientDTO)
        {
            _logger.LogInformation("Adding client {@ClientDTO}", clientDTO);

            var client = _mapper.Map<Client>(clientDTO);
            await _clientRepository.AddClientAsync(client);

            _logger.LogInformation("Client added successfully");
        }

        public List<string> GetLastSearchParameters()
        {
            var lastSearchParameters = _clientRepository.GetLastSearchParameters();
            _logger.LogInformation("Last search parameters: {@LastSearchParameters}", lastSearchParameters);
            return lastSearchParameters;
        }
    }
}

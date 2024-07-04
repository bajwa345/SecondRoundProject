using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondRoundProject.DTOs;
using SecondRoundProject.Helpers;
using SecondRoundProject.Services;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(IClientService clientService, ILogger<ClientController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }

    [HttpGet("listclients")]
    public async Task<IActionResult> GetClients([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "Id", [FromQuery] string sortOrder = "ASC")
    {
        try
        {
            var clients = await _clientService.GetClientsAsync(filter, page, pageSize, sortBy, sortOrder);
            var pagination = PaginationHelper.CreatePaginationHeader(page, pageSize, clients.TotalCount);
            Response.Headers.Add("X-Pagination", pagination);
            return Ok(clients.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving clients.");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("suggestions")]
    public IActionResult GetSuggestions()
    {
        var suggestions = _clientService.GetLastSearchParameters();
        return Ok(suggestions);
    }

    [HttpPost("newclient")]
    public async Task<IActionResult> AddClient([FromBody] ClientDTO clientDTO)
    {
        try { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            clientDTO.CreatedBy = int.Parse(User?.FindFirstValue(ClaimTypes.NameIdentifier)??"0");
            await _clientService.AddClientAsync(clientDTO);
            return Ok(new { Message = "Client added successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding the client.");
            return StatusCode(500, "Internal server error");
        }
    }
}

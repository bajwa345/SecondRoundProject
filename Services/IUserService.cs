using SecondRoundProject.DTOs;

namespace SecondRoundProject.Services
{
    public interface IUserService
    {
        Task<(bool, string)> RegisterAsync(RegisterDTO model);
        Task<(bool, string, string?)> LoginAsync(LoginDTO model);
    }

}
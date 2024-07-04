using SecondRoundProject.Models;

namespace SecondRoundProject.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByUsernameAsync(string username);
        Task<int?> CreateUserAsync(ApplicationUser user);
    }

}

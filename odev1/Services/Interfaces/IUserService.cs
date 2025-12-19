using odev1.Models;

namespace odev1.Services
{
    public interface IUserService
    {
        Task<UserDetails> GetByUsernameAsync(string username);
        Task<UserDetails> GetByEmailAsync(string email);
        Task<UserDetails> updateProfileAsync(string userId, string firstName, string lastName, string phoneNumber);
    }
}


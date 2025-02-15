using BusinessObject.Entities;

namespace API.Services
{
    public interface IGoogleLoginService
    {
        Task<User> GetUserByGoogleIdAsync(Guid googleId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);

    }
}

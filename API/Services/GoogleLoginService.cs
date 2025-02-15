using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class GoogleLoginService(IRepository<User> _userRepository) : IGoogleLoginService
    {

        public async Task<User> GetUserByGoogleIdAsync(Guid googleId)
        {
            return await _userRepository.Find(g => g.Id == googleId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.Find(u => u.Email.Equals(email)).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();
            return user;
        }
    }
}

using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<UserResponse> GetUser(Guid id);
        Task<PagedResponse<List<UserListResponse>>> GetUsers(PagedRequest request);
        Task<User> GetByEmailAsync(string email);
        Task<string> GenerateCodeConfirmEmail(Guid userId);
        Task<bool> ConfirmEmail(Guid userId);
        Task<bool> ChangeIsDeletedUser(Guid userId, bool isDeleted);
        Task<bool> ChangePassword(Guid userId, string newHashPassword);
        Task<bool> IsVerifyCode(Guid userId, string code);
        Task<bool> UpdateUser(User user);
        Task<bool> SaveRefreshToken(Guid userId, string refreshToken, DateTime ExpirationDate);
        Task<User> GetUserByRefreshToken(string refreshToken);
    }
}

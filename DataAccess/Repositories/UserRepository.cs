using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using BusinessObject.Wrappers;
using DataAccess.Context;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository(ApplicationDbContext _context)
        : Repository<User>(_context), IUserRepository
    {
        public async Task<bool> ChangePassword(Guid userId, string newHashPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            user.PasswordHash = newHashPassword;

            return true;
        }

        public async Task<bool> ConfirmEmail(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            user.IsEmailConfirmed = true;

            return true;
        }

        public async Task<bool> IsVerifyCode(Guid userId, string code)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            var confirm = await _context.EmailConfirmationTokens
                .FirstOrDefaultAsync(e => e.UserId == userId && e.Token.Equals(code) && e.ExpirationDate >= DateUtility.GetCurrentDateTime());

            return confirm != null;
        }

        public async Task<string> GenerateCodeConfirmEmail(Guid userId)
        {
            string code = Util.Generate6DigitCode();

            await _context.EmailConfirmationTokens.AddAsync(new EmailConfirmationToken
            {
                Id = Guid.NewGuid(),
                ExpirationDate = DateUtility.GetCurrentDateTime().AddMinutes(5),
                Token = code,
                UserId = userId
            });

            return code;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<bool> UpdateUser(User u)
        {
            var user = await _context.Users.FindAsync(u.Id);

            if (user == null) return false;

            user.FullName = u.FullName;
            user.Phone = u.Phone;
            user.Address = u.Address;
            user.Email = u.Email;
            user.RoleId = u.RoleId;

            return true;
        }

        public async Task<bool> SaveRefreshToken(Guid userId, string refreshToken, DateTime ExpirationDate)
        {
            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                ExpirationDate = ExpirationDate,
                UserId = userId
            });

            return true;
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(t => t.User)
                .ThenInclude(t => t.Role)
                .FirstOrDefaultAsync(t => t.Token.ToLower().Equals(refreshToken.ToLower()) 
                && t.ExpirationDate >= DateUtility.GetCurrentDateTime());

            return token?.User;
        }

        public async Task<bool> ChangeIsDeletedUser(Guid userId, bool isDeleted)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return false;

            user.IsDeleted = isDeleted;

            return true;
        }

        public async Task<PagedResponse<List<UserListResponse>>> GetUsers(PagedRequest request)
        {
            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Role)
                .OrderBy(u => u.FullName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);

            var users = await query
                .Select(u => new UserListResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.Name
                })
                .ToListAsync();

            var totalRecords = await _context.Users
                .CountAsync(u => !u.IsDeleted);

            var response = new PagedResponse<List<UserListResponse>>(
                users,
                request.PageNumber,
                request.PageSize,
                (int)Math.Ceiling((double)totalRecords / request.PageSize)
            );

            return response;
        }
        public async Task<UserResponse> GetUser(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    IsDeleted = u.IsDeleted,
                    RoleId = u.RoleId,
                    Role = u.Role.Name
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return user;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public int RoleId { get; set; }
        public string Role { get; set; }
        public string Avatar {  get; set; }
    }
}

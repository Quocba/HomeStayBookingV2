using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RegisterDTO
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(6)]
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}

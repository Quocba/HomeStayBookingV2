using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class AddFacilityDTO
    {
        public string Name { get; set; }

        [MaxLength(255)]
        [Required]
        public string Description { get; set; }
    }
}

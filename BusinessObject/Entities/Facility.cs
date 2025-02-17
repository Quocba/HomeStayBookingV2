using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;
using BusinessObject.Shares;

namespace BusinessObject.Entities
{
    [Table("Facility")]
    public class Facility : BaseEntity<Guid>
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(255)]
        [Required]
        public string Description { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime? UpdateAt { get; set; }
    }
}

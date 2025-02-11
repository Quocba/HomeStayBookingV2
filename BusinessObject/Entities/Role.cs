using BusinessObject.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class Role : BaseEntity<int>
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }

}

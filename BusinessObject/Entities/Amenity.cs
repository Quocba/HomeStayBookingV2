using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("Amenity")]
    public class Amenity : BaseEntity<Guid>
    {
        public string Name {  get; set; }
        public bool isDeleted { get; set; } = false;

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Common
{
    public abstract class BaseEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }
    }
}

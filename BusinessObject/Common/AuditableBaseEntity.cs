using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Shares;

namespace BusinessObject.Common
{
    public abstract class AuditableBaseEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateUtility.GetCurrentDateTime();
        public Guid LastModifiedById { get; set; }
        public DateTime? LastModifiedAt { get; set; } = DateUtility.GetCurrentDateTime();
    }
}

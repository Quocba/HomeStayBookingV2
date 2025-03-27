using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Entities
{
    public class TTlockAccuontDTO
    {
        [JsonIgnore(Condition =JsonIgnoreCondition.Always)]
        public Guid TTLockID { get; set; } = Guid.NewGuid();

        public string? TTLockUserName { get; set; }
        public string? Password { get; set; }

        public Guid? HomeStayID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class EditAmentityDTO
    {
        public Guid AmentityID { get; set; }

        public string? AmentityName { get;set; }

        public string? Description {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class DeleteHomeStayImageDTO
    {
        public List<Guid> ImageIds { get; set; } = new List<Guid>();
    }
}


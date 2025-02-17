using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class DeleteListDTO
    {
      public List<Guid> ID {  get; set; } = new List<Guid>();
    }
}

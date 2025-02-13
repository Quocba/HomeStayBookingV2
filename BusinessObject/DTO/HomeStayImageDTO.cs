using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.DTO
{
    public class HomeStayImageDTO
    {
        public Guid HomeStayID { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}

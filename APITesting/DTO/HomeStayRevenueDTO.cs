using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting.DTO
{
    public class HomeStayRevenueDTO
    {
        public Guid HomeStayID { get; set; }
        public string HomeStayName { get; set; }
        public string MainImage { get; set; }
        public string Address { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

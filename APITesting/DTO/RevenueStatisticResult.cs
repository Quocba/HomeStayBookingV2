using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting.DTO
{
    public class RevenueStatisticResult
    {
        public string Month { get; set; }
        public int Booking { get; set; }
        public decimal Revenue { get; set; }
    }
}

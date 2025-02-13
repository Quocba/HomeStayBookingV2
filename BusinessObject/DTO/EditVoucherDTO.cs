using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class EditVoucherDTO
    {
        public string Image { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int QuantityUsed { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}

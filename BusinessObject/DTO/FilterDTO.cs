using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class FilterDTO
    {
       public List<string>? AmenityNames {  get; set; }
       public decimal? MinPrice {  get; set; }
       public decimal? MaxPrice { get; set; }
       public List<int>? Standard {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Entities;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.DTO
{
    public class AddHomeStayRequest
    {
        public IFormFile MainImage { get; set; }
        public string Name {  get; set; }
        public int OpenIn {  get; set; }
        public string Description {  get; set; }
        public int Standar {  get; set; }
        public bool isDeleted { get; set; } = false;
        public string Address {  get; set; }
        public string City { get; set; }
        public bool isBlocked {  get; set; } = false;
        public string CheckInTime {  get; set; }
        public string CheckOutTime { get; set; }
        public List<IFormFile> Images { get; set; }
        
        public DateTime Date {  get; set; }
        public Decimal Price {  get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

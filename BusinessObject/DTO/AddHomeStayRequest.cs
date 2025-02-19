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
        public string MainImage { get; set; }
        public string Name {  get; set; }
        public int OpenIn {  get; set; }
        public string Description {  get; set; }
        public int Standar {  get; set; }
        public string Address {  get; set; }
        public string City { get; set; }
        public bool isBlocked {  get; set; } = false;
        public string CheckInTime {  get; set; }
        public string CheckOutTime { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        
        public DateTime Date {  get; set; }        
        public bool IsDeleted { get; set; } = false;
    }
}

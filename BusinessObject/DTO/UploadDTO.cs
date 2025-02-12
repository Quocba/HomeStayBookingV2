using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessObject.DTO
{
    public class UploadDTO
    {
       public IFormFile File { get; set; }
    }
}

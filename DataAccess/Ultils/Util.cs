using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
namespace BusinessObject.Shares
{
    public static class Util
    {
        private static readonly Random random = new();

        public static string Generate6DigitCode()
        {
            int code = random.Next(100000, 1000000);
            return code.ToString();
        }

        public static string SaveImage(IFormFile image, IWebHostEnvironment environment)
        {
            if (image == null || environment == null)
            {
                throw new ArgumentNullException("Invalid image or environment settings.");
            }

            string uploadsFolder = Path.Combine(environment.ContentRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = "";
            if (image.FileName == null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.Name);
            }
            uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            return "/images/" + uniqueFileName;
        }

    }
}

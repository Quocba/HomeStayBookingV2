using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        public static string GenerateRandomString(int length = 6, bool includeNumbers = false)
        {
            Random random = new Random();
            string chars = includeNumbers
                ? "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
                : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

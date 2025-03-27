using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Entities;
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

        public static string GenerateMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert to hexadecimal string
                var sb = new System.Text.StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // "x2" => lowercase hex
                }

                return sb.ToString();
            }
        }

    }
}

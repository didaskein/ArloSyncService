using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArloSyncService.Common.Extensions
{
    public static class StringExtensions
    {

        public static string GetBase64(this string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        public static string WrapText(this string text, int width)
        {
            for (int i = width; i < text.Length; i += width + 1)
            {
                text = text.Insert(i, "\n");
            }
            return text;
        }

        public static string FormatCert(this string cert)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN CERTIFICATE-----");
            sb.AppendLine(cert.WrapText(64));
            sb.AppendLine("-----END CERTIFICATE-----");
            return sb.ToString();
        }


        public static string GetRandomIdForUser(this string userId)
        {
            var random = new Random();
            string randomDigits = new string(Enumerable.Range(0, 10).Select(_ => random.Next(0, 10).ToString()[0]).ToArray());
            var userName = $"user_{userId}_{randomDigits}";
            return userName;
        }



    }

}

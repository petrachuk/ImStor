using System;
using System.Security.Cryptography;
using System.Text;

namespace ImStor
{
    public static class Extensions
    {
        public static Guid GetMd5Hash(this byte[] input)
        {
            using var md5Hash = MD5.Create();

            var data = md5Hash.ComputeHash(input);

            var sBuilder = new StringBuilder();

            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return new Guid(sBuilder.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.Helpers
{
    public class EncryptHelper
    {
        public static string SHA256Encrypt(string data)
        {
            var sha256 = new SHA256Managed();
            var tmpByte = Encoding.UTF8.GetBytes(data);
            var hash = sha256.ComputeHash(tmpByte);
            sha256.Clear();
            var builder = new StringBuilder(64);
            foreach (var b in hash)
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }
    }
}

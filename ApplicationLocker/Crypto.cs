using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLocker
{
    public class Crypto
    {
        public static string Encrypt(string text, string key)
        {
            string result = string.Empty;
            for (int c = 0; c < text.Length; c++)
                result += (char)(text[c] ^ key[(c % key.Length)]);
            return result;
        }
        public static string Decrypt(string text, string key)
        {
            return Encrypt(text, key);
        }

        public static string Hash(string text)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

    }
}

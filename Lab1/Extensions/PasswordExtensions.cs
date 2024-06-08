using System.Security.Cryptography;
using System.Text;

namespace Lab1.Extensions
{
    public static class PasswordExtensions
    {
        public static string MD5Hash(string text)
        {
            var md5 = MD5.Create();

            //compute hash from the bytes of text  
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash!;

            StringBuilder strBuilder = new();

            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}

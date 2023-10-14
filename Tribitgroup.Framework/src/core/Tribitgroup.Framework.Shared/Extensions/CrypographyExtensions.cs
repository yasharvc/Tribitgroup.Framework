using System.Security.Cryptography;
using System.Text;

namespace Tribitgroup.Framework.Shared.Extensions
{
    public static class CrypographyExtensions
    {
        public static string ToSHA256(this string exp)
        {
            var byteArrayResultOfRawData =
                  Encoding.UTF8.GetBytes(exp);

            var byteArrayResult = SHA256.HashData(byteArrayResultOfRawData);
            return string.Concat(Array.ConvertAll(byteArrayResult, h => h.ToString("X2")));
        }
        public static string ToSHA512(this string exp)
        {
            var byteArrayResultOfRawData =
                  Encoding.UTF8.GetBytes(exp);

            var byteArrayResult = SHA512.HashData(byteArrayResultOfRawData);
            return string.Concat(Array.ConvertAll(byteArrayResult, h => h.ToString("X2")));
        }
    }
}
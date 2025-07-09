using System.Security.Cryptography;
using System.Text;

namespace Timesheets_APP.Helpers // <-- match your actual namespace
{
    public static class SecurityHelper
    {
        public static string Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes); // requires .NET 5 or later
        }
    }
}

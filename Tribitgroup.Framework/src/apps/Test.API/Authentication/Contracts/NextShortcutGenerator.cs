
using System.Security.Cryptography;

namespace Test.API.Authentication.Contracts
{
    public class NextShortcutGenerator : INextShortcutGenerator
    {
        const string Chars = "ABCDEFGHKMNPQRSTWXYZ23456789";
        public Task<string> GetNextShortcutAsync()
        {
            byte[] randomNumber = new byte[6];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var res = "";
            for (int i = 0; i < 6; i++)
            {
                int index = BitConverter.ToInt32(randomNumber, 0) % Chars.Length;
                res += Chars[index];
            }

            return Task.FromResult(res);
        }
    }
}

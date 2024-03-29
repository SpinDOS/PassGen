using System;
using System.Security.Cryptography;
using System.Text;

namespace PassGen.Lib
{
    public sealed class PasswordGenerator
    {
        public string GeneratePassword(string target, string salt)
        {
            target = target ?? throw new ArgumentNullException(nameof(target));
            salt = salt ?? throw new ArgumentNullException(nameof(salt));

            var stringToHash = $"{target}@{salt}";
            var generatedPassword = CalculateSha512Hash(stringToHash);

            TakeFirst8Chars(generatedPassword);
            EnrichWithNecessaryCharTypes(generatedPassword);

            return generatedPassword.ToString();
        }

        private StringBuilder CalculateSha512Hash(string stringToHash)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(stringToHash);

            byte[] hashBytes;
            using (var sha512 = SHA512.Create())
                hashBytes = sha512.ComputeHash(bytesToHash);

            var hashString = new StringBuilder(128);
            foreach (var hashByte in hashBytes)
                hashString.AppendFormat("{0:x2}", hashByte);

            return hashString;
        }

        private void TakeFirst8Chars(StringBuilder generatedPassword)
        {
            generatedPassword.Remove(8, generatedPassword.Length - 8);
        }

        private void EnrichWithNecessaryCharTypes(StringBuilder generatedPassword)
        {
            generatedPassword.Insert(0, 'p');
            generatedPassword.Append("#7G");
        }
    }
}

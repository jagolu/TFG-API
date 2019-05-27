using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;

namespace API.Util
{
    public static class PasswordHasher
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public static string hashPassword(string pass)
        {
            if (pass == null) return null;
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pass,
                salt: new byte[int.Parse(_configuration["Crypt:saltSize"])],
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: int.Parse(_configuration["Crypt:hashCount"]),
                numBytesRequested: int.Parse(_configuration["Crypt:subkeyLength"])
            )); ;
        }

        public static bool areEquals(string pass, string hashPass)
        {
            if(hashPassword(pass) != hashPass)
            {
                return false;
            }

            return true;
        }

        public static bool validPassword(string pass)
        {
            if (pass == null) return false;
            if (pass.Length < 8 || pass.Length > 20 || !Regex.IsMatch(pass, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$"))
            {
                return false;
            }

            return true;
        }
    }
}

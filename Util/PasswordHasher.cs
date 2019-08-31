using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.RegularExpressions;

namespace API.Util
{
    public static class PasswordHasher
    {
        //
        // ──────────────────────────────────────────────────────────────────────
        //   :::::: C L A S S   V A R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────
        //

        /// <value>The configuration of the </value>
        private static IConfiguration _configuration;


        //
        // ──────────────────────────────────────────────────────────────────────────
        //   :::::: C O N S T R U C T O R S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration of the app</param>
        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //
        // ──────────────────────────────────────────────────────────────────────────────────
        //   :::::: P U B L I C   F U N C T I O N S : :  :   :    :     :        :          :
        // ──────────────────────────────────────────────────────────────────────────────────
        //

        /// <summary>
        /// Get the password hashed
        /// </summary>
        /// <param name="pass">The password without hash</param>
        /// <returns>The password hashed</returns>
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

        /// <summary>
        /// Check if both password are equals
        /// </summary>
        /// <param name="pass">The password without hash</param>
        /// <param name="hashPass">The password hashed</param>
        /// <returns>True if both password are equals, false otherwise</returns>
        public static bool areEquals(string pass, string hashPass)
        {
            if(hashPassword(pass) != hashPass)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a password is valid
        /// </summary>
        /// <param name="pass">The password without hash</param>
        /// <returns>True if the password is valid, false otherwise</returns>
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

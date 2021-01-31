using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.Threading.Tasks;

namespace User_Management_System.Models
{
    public class Encryptor
    {
        public string encrypt(string password)
        {
            string saltString = "memelord";
            var salt = Encoding.UTF8.GetBytes(saltString);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}

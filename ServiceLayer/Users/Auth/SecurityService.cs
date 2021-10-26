using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ServiceLayer.Users
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
        Guid CreateCryptographicallySecureGuid();
    }

    public class SecurityService: ISecurityService
    {
        private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

        public string GetSha256Hash(string input)
        {
            using (var hashAlgorithm=new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public Guid CreateCryptographicallySecureGuid()
        {
            var bytes = new byte[16];
            _random.GetBytes(bytes);
            return new Guid(bytes);
        }

    }
}

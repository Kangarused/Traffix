using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Traffix.Common.IocAttributes;

namespace Traffix.Common.Providers
{
    public interface ICryptoProvider
    {
        byte[] EncodeMessage(string message);
        string DecodeMessage(byte[] message);
        string GetHash(string input);
        bool VerifyHash(string hash, string input);
    }


    [Singleton]
    public class CryptoProvider : ICryptoProvider
    {
        private readonly IConfigurationManagerProvider _configurationManagerProvider;

        private static Random _rnd = new Random();

        private string Secret
        {
            get
            {
                return _configurationManagerProvider.GetConfigValue("Secret");
            }
        }
        private const int AesKeySizeInBits = 128;
        private const int Rfc2898Iterations = 1000;




        public CryptoProvider(IConfigurationManagerProvider configurationManagerProvider)
        {
            _configurationManagerProvider = configurationManagerProvider;
        }

        private const int SaltSize = 16;

        public byte[] EncodeMessage(string message)
        {
            using (var aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int keyStrengthInBytes = aes.KeySize / 8;

                var salt = new byte[SaltSize];
                _rnd.NextBytes(salt);


                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Secret, salt, Rfc2898Iterations);
                aes.Key = rfc2898.GetBytes(keyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(keyStrengthInBytes);

                byte[] rawPlaintext = Encoding.Unicode.GetBytes(message);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawPlaintext, 0, rawPlaintext.Length);
                    }

                    var result = new List<byte>();
                    result.AddRange(salt);
                    result.AddRange(ms.ToArray());
                    var cipherText = result.ToArray();
                    return cipherText;
                }
            }
        }

        public string DecodeMessage(byte[] message)
        {
            using (var aes = new AesManaged())
            {

                byte[] salt = new byte[SaltSize];
                Buffer.BlockCopy(message, 0, salt, 0, SaltSize);
                
                byte[]cipherText  = new byte[message.Length - SaltSize];
                Buffer.BlockCopy(message, SaltSize, cipherText, 0, message.Length - SaltSize);


                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                int keyStrengthInBytes = aes.KeySize / 8;

                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Secret, salt, Rfc2898Iterations);
                aes.Key = rfc2898.GetBytes(keyStrengthInBytes);
                aes.IV = rfc2898.GetBytes(keyStrengthInBytes);
                
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherText, 0, cipherText.Length);
                    }
                    var plainText = ms.ToArray();
                    return Encoding.Unicode.GetString(plainText);
                }
            }
        }

        public string GetHash(string input)
        {
            return BCrypt.Net.BCrypt.HashPassword(input, 10);
        }

        public bool VerifyHash(string hash, string input)
        {
            return BCrypt.Net.BCrypt.Verify(input,hash);
        }
    }
}

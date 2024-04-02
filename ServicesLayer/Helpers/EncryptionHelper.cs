using System.Security.Cryptography;
using System.Text;

namespace ServicesLayer.Helpers
{
    public static class EncryptionHelper
    {
        // AES encryption key and IV (Initialization Vector)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwx"); // Key must be 16, 24, or 32 bytes long
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // IV must be 16 bytes long

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(byte[] encryptedBytes)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

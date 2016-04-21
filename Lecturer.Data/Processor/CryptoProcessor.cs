using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lecturer.TestCreator
{
    public class CryptoProcessor
    {
        //private static string key = "somekey";
        private static Encoding enco = Encoding.GetEncoding("cp866");

        static byte[] EncryptBytes(string data, string key)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateEncryptor(
                (new PasswordDeriveBytes(key, null)).GetBytes(16),
                new byte[16]);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);

            var array = ToByteArray(data);

            cs.Write(array, 0, array.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();            
        }
        public static string Encrypt(string data, string key)
        {
            return Convert.ToBase64String(EncryptBytes(data, key));
        }

        public static string Decrypt(string data, string password)
        {
            try
            {
                CryptoStream cs = InternalDecrypt(Convert.FromBase64String(data), password);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch
            {
                return data;
            }
        }

        static CryptoStream InternalDecrypt(byte[] data, string password)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            ICryptoTransform ct = sa.CreateDecryptor(
                (new PasswordDeriveBytes(password, null)).GetBytes(16),
                new byte[16]);

            MemoryStream ms = new MemoryStream(data);
            return new CryptoStream(ms, ct, CryptoStreamMode.Read);
        }

        private static string ToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private static byte[] ToByteArray(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }
    }
}

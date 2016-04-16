using System;
using System.Security.Cryptography;
using System.Text;

namespace Lecturer.TestCreator
{
    /// <summary>
    /// Шифрование и дешифрование данных
    /// </summary>
    public class CryptoProcessor
    {
        private static string key = "<RSAKeyValue><Modulus>p0E8A/6qB4NGUj8NdALuKeBo5RsYezkTbt6bG3895sHXiFq9BwvwprMP9Ue61mpNpitSjDBG77QS/Ctmc0jOTy/+UTzBTLacMq3n51z5s7eGUMxY+fzfsmih6Ncl/DrGpfh+FhRzxTjtsWmOBNxoV6Dwy+BpsGaoBgBkUzeqZ70=</Modulus><Exponent>AQAB</Exponent><P>1ZsjX4Rzuhz8mX6u5BiAci6kbGYPyKLd5N4hpybHkCZYGHxd2eQr4gRH7Gp/2dBNojvDzjRD7peHRCaUkpdjsQ==</P><Q>yHMXmNtNrnuqEL146cQDi9cVgPlMqQjTo5elEmBe6FfY777Tu5yMWBgaa+4x3bFzu3nBeNvB39RUZovVWiyDzQ==</Q><DP>ZxKdVxIK5dvm6AqBSf+ou3BWVxhItYAhoratdoL3+U8HY4lfoCzCICYArswVNX2WeJpuOapuvUrRMsmLF9GFgQ==</DP><DQ>EUydhLuogJ57luZDQSmBhNgTKwZY712rpjq4LFXU2wh52HcHnvFry06JOTddZlyiOFPRtrSAjuisQA1hZF7jIQ==</DQ><InverseQ>bt6Lgljo/F9ANVnALPrds/kEy7x7VJxYYYhc626brHqmQtKZKh5kvaWblQT4A0gyfxTFFi1MMsAztdYgzuVgtg==</InverseQ><D>A9ymf1wdvnMqSENi8uMPb0GagnHD+LJqb7Stpa6kNgQTTzdzJmrA6YR4cZwwpPtK5DObYhfKR4YjqxVwdeiANPvR7J2zF+WbANxj1pY+IQg6PyGSZ3AbqQqEElyXOo3UIPkcPkztHUdz+0ylK9COD3mpcHmhf6You3yDkWS0mlE=</D></RSAKeyValue>";
        private static Encoding enco = Encoding.GetEncoding("cp866");


        /// <summary>
        /// Шифрование строки, которая будет сохранена в xml
        /// </summary>
        /// <param name="data">нешифрованная строка</param>
        /// <returns>шифрованная строка</returns>
        public static string Encrypt(string data)
        {
            byte[] encContent;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(key);
            encContent = rsa.Encrypt(ToByteArray(data), true);
            return Convert.ToBase64String(encContent);
        }

        /// <summary>
        /// Дешифрование строки, извлеченной из файла
        /// </summary>
        /// <param name="data">шифрованная строка</param>
        /// <returns>нешифрованная строка</returns>
        public static string Decrypt(string data)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(key);
                var decryptedData = rsa.Decrypt(Convert.FromBase64String(data), true);

                return ToString(decryptedData);
            }
            catch
            {
                return data;
            }
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

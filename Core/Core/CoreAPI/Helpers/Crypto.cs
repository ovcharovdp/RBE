using System;
using System.Security.Cryptography;
using System.Text;

namespace CoreAPI.Helpers
{
    /// <summary>
    /// Пространство имен для реализации вспомогательных функций.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }
    /// <summary>
    /// Вспомогательные методы криптографии
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// Шифрование текста в md5
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <returns>Шифрованный текст в md5</returns>
        public static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// Сравнение строки с хэшом md5 на равность
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <param name="hash">Зашифрованная в MD5 строка</param>
        /// <returns>Признак совпадения</returns>
        public static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = Crypto.GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}

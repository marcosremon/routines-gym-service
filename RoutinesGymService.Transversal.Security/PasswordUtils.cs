using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace RoutinesGymService.Transversal.Security
{
    public class PasswordUtils
    {
        private readonly byte[] _secretkeyByte;
        private readonly byte[] _publickeybyte;

        public PasswordUtils(IConfiguration configuration)
        {
            string publicKey = configuration["Password:publicKey"]!;
            string secretKey = configuration["Password:secretKey"]!;

            _secretkeyByte = Encoding.UTF8.GetBytes(secretKey);
            _publickeybyte = Encoding.UTF8.GetBytes(publicKey);
        }

        public byte[] PasswordEncoder(string password)
        {
            try
            {
                string textToEncrypt = password;
                byte[] inputbyteArray = Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] result;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _publickeybyte;
                    aes.IV = _secretkeyByte;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                            cs.FlushFinalBlock();
                            result = ms.ToArray();
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public string PasswordDecoder(byte[] encryptedPassword)
        {
            try
            {
                byte[] result;
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _publickeybyte;
                    aes.IV = _secretkeyByte;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedPassword, 0, encryptedPassword.Length);
                            cs.FlushFinalBlock();
                            result = ms.ToArray();
                        }
                    }
                }
                return Encoding.UTF8.GetString(result);
            }
            catch (CryptographicException ex)
            {
                throw new Exception("Error al desencriptar la contraseña. Asegúrate de que los datos encriptados y las claves sean correctos.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public static bool IsPasswordEncrypted(string password)
        {
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(password);
                return decodedBytes.Length % 16 == 0;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8 &&
               password.Any(char.IsLower) &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsDigit) &&
               password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
    }
}
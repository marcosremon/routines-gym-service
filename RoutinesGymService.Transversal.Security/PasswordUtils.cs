using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace RoutinesGymService.Transversal.Security
{
    public class PasswordUtils
    {
        private readonly IConfiguration _configuration;
        private readonly int _iterations;
        private readonly int _saltSize;
        private readonly int _keySize;
        private readonly int _nonceSize;
        private readonly int _tagSize;

        public PasswordUtils(IConfiguration configuration)
        {
            _configuration = configuration;
            _iterations = int.Parse(configuration["Password:KeyDerivation:Iterations"] ?? "100000");
            _saltSize = int.Parse(configuration["Password:KeyDerivation:SaltSize"] ?? "16");
            _keySize = int.Parse(configuration["Password:KeyDerivation:KeySize"] ?? "32");
            _nonceSize = int.Parse(configuration["Password:Encryption:NonceSize"] ?? "12");
            _tagSize = int.Parse(configuration["Password:Encryption:TagSize"] ?? "16");
        }

        public byte[] PasswordEncoder(string password)
        {
            try
            {
                byte[] salt = RandomNumberGenerator.GetBytes(_saltSize);
                byte[] key = DeriveKey(password, salt);
                byte[] nonce = RandomNumberGenerator.GetBytes(_nonceSize);
                byte[] encryptedData = EncryptWithAesGcm(key, nonce, Encoding.UTF8.GetBytes(password));

                byte[] result = new byte[salt.Length + nonce.Length + encryptedData.Length];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
                Buffer.BlockCopy(nonce, 0, result, salt.Length, nonce.Length);
                Buffer.BlockCopy(encryptedData, 0, result, salt.Length + nonce.Length, encryptedData.Length);

                return result;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Error al cifrar la contraseña", ex);
            }
        }

        public byte[] PasswordEncoder(string password, bool isGoogleLogin)
        {
            // Si es login de Google, permite email como contraseña sin validación adicional
            if (isGoogleLogin && password.Contains('@'))
            {
                return PasswordEncoder(password);
            }

            // Si no es Google login, usa el método normal
            return PasswordEncoder(password);
        }

        public bool VerifyPassword(byte[] encryptedPassword, string plainPassword)
        {
            try
            {
                byte[] salt = new byte[_saltSize];
                byte[] nonce = new byte[_nonceSize];
                byte[] cipherText = new byte[encryptedPassword.Length - _saltSize - _nonceSize];

                Buffer.BlockCopy(encryptedPassword, 0, salt, 0, _saltSize);
                Buffer.BlockCopy(encryptedPassword, _saltSize, nonce, 0, _nonceSize);
                Buffer.BlockCopy(encryptedPassword, _saltSize + _nonceSize, cipherText, 0, cipherText.Length);

                byte[] key = DeriveKey(plainPassword, salt);

                try
                {
                    byte[] decryptedData = DecryptWithAesGcm(key, nonce, cipherText);
                    return Encoding.UTF8.GetString(decryptedData) == plainPassword;
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyPassword(string encryptedPasswordBase64, string plainPassword)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedPasswordBase64);
            return VerifyPassword(encryptedBytes, plainPassword);
        }

        private byte[] DeriveKey(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(
                password,
                salt,
                _iterations,
                HashAlgorithmName.SHA256);

            return deriveBytes.GetBytes(_keySize);
        }

        private byte[] EncryptWithAesGcm(byte[] key, byte[] nonce, byte[] plaintext)
        {
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[_tagSize];

            using var aesGcm = new AesGcm(key, _tagSize);
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

            byte[] result = new byte[ciphertext.Length + tag.Length];
            Buffer.BlockCopy(ciphertext, 0, result, 0, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, result, ciphertext.Length, tag.Length);

            return result;
        }

        private byte[] DecryptWithAesGcm(byte[] key, byte[] nonce, byte[] ciphertextWithTag)
        {
            byte[] ciphertext = new byte[ciphertextWithTag.Length - _tagSize];
            byte[] tag = new byte[_tagSize];

            Buffer.BlockCopy(ciphertextWithTag, 0, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(ciphertextWithTag, ciphertext.Length, tag, 0, tag.Length);

            byte[] plaintext = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(key, _tagSize);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

            return plaintext;
        }

        public static string GenerateSecurePassword()
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            int length = 16;

            if (length < 12) length = 12;

            using var rng = RandomNumberGenerator.Create();

            var password = new char[length];
            byte[] randomBytes = new byte[length];

            rng.GetBytes(randomBytes);

            password[0] = uppercase[randomBytes[0] % uppercase.Length];
            password[1] = lowercase[randomBytes[1] % lowercase.Length];
            password[2] = digits[randomBytes[2] % digits.Length];
            password[3] = special[randomBytes[3] % special.Length];

            string allChars = uppercase + lowercase + digits + special;

            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[randomBytes[i] % allChars.Length];
            }

            for (int i = 0; i < length; i++)
            {
                int swapIndex = randomBytes[i] % length;
                (password[i], password[swapIndex]) = (password[swapIndex], password[i]);
            }

            return new string(password);
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 9)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace RoutinesGymService.Transversal.Security
{
    public class PasswordUtils
    {
        private readonly int _iterations;
        private readonly int _saltSize;
        private readonly int _keySize;
        private readonly int _nonceSize;
        private readonly int _tagSize;
        private readonly byte[] _masterKey;

        public PasswordUtils(IConfiguration configuration)
        {
            _iterations = int.Parse(configuration["Password:KeyDerivation:Iterations"] ?? "100000");
            _saltSize = int.Parse(configuration["Password:KeyDerivation:SaltSize"] ?? "16");
            _keySize = int.Parse(configuration["Password:KeyDerivation:KeySize"] ?? "32");
            _nonceSize = int.Parse(configuration["Password:Encryption:NonceSize"] ?? "12");
            _tagSize = int.Parse(configuration["Password:Encryption:TagSize"] ?? "16");

            string masterKeyString = configuration["Password:MasterKey"] ?? throw new Exception("Password:MasterKey not configured");
            if (masterKeyString.Length != 32)
                throw new Exception("MasterKey must be exactly 32 characters");

            _masterKey = Encoding.UTF8.GetBytes(masterKeyString);
        }

        public byte[] PasswordEncoder(string password)
        {
            try
            {
                byte[] masterNonce = RandomNumberGenerator.GetBytes(_nonceSize);
                byte[] encryptedPassword = EncryptWithAesGcm(_masterKey, masterNonce, Encoding.UTF8.GetBytes(password));

                byte[] marker = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

                byte[] result = new byte[masterNonce.Length + encryptedPassword.Length + marker.Length];
                Buffer.BlockCopy(masterNonce, 0, result, 0, masterNonce.Length);
                Buffer.BlockCopy(encryptedPassword, 0, result, masterNonce.Length, encryptedPassword.Length);
                Buffer.BlockCopy(marker, 0, result, masterNonce.Length + encryptedPassword.Length, marker.Length);

                return result;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Error al cifrar la contraseña", ex);
            }
        }

        public byte[] PasswordEncoder(string password, bool isGoogleLogin)
        {
            return PasswordEncoder(password);
        }

        public bool VerifyPassword(byte[] encryptedPassword, string plainPassword)
        {
            try
            {
                if (HasMasterKeyLayer(encryptedPassword))
                {
                    try
                    {
                        string decryptedPassword = DecryptPasswordWithMasterKeyStatic(
                            encryptedPassword,
                            Encoding.UTF8.GetString(_masterKey)
                        );
                        return decryptedPassword == plainPassword;
                    }
                    catch (CryptographicException)
                    {
                        return false;
                    }
                }
                else
                {
                    return VerifyOldFormatPassword(encryptedPassword, plainPassword);
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

        public static string DecryptPasswordWithMasterKeyStatic(byte[] encryptedPassword, string masterKey)
        {
            int nonceSize = 12;
            int tagSize = 16;

            if (masterKey.Length != 32)
                throw new ArgumentException("MasterKey must be 32 characters");

            byte[] masterKeyBytes = Encoding.UTF8.GetBytes(masterKey);

            if (encryptedPassword.Length < 4 + nonceSize + tagSize ||
                encryptedPassword[^4] != 0xDE ||
                encryptedPassword[^3] != 0xAD ||
                encryptedPassword[^2] != 0xBE ||
                encryptedPassword[^1] != 0xEF)
            {
                throw new CryptographicException("Esta contraseña no tiene capa de descifrado maestro");
            }

            int masterNonceOffset = 0;
            int masterEncryptedOffset = nonceSize;
            int masterEncryptedLength = encryptedPassword.Length - nonceSize - 4;

            byte[] masterNonce = new byte[nonceSize];
            byte[] masterEncrypted = new byte[masterEncryptedLength];

            Buffer.BlockCopy(encryptedPassword, masterNonceOffset, masterNonce, 0, nonceSize);
            Buffer.BlockCopy(encryptedPassword, masterEncryptedOffset, masterEncrypted, 0, masterEncryptedLength);

            if (masterEncrypted.Length < tagSize)
                throw new CryptographicException("Datos cifrados corruptos - longitud insuficiente");

            byte[] ciphertext = new byte[masterEncrypted.Length - tagSize];
            byte[] tag = new byte[tagSize];

            Buffer.BlockCopy(masterEncrypted, 0, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(masterEncrypted, ciphertext.Length, tag, 0, tagSize);

            // Descifrar
            byte[] plaintext = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(masterKeyBytes, tagSize);
            aesGcm.Decrypt(masterNonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }

        private bool VerifyOldFormatPassword(byte[] encryptedPassword, string plainPassword)
        {
            try
            {
                byte[] salt = new byte[_saltSize];
                byte[] nonce = new byte[_nonceSize];

                if (encryptedPassword.Length < _saltSize + _nonceSize + _tagSize)
                    return false;

                byte[] cipherText = new byte[encryptedPassword.Length - _saltSize - _nonceSize];

                Buffer.BlockCopy(encryptedPassword, 0, salt, 0, _saltSize);
                Buffer.BlockCopy(encryptedPassword, _saltSize, nonce, 0, _nonceSize);
                Buffer.BlockCopy(encryptedPassword, _saltSize + _nonceSize, cipherText, 0, cipherText.Length);

                byte[] key = DeriveKey(plainPassword, salt);

                byte[] decryptedData = DecryptWithAesGcm(key, nonce, cipherText);
                return Encoding.UTF8.GetString(decryptedData) == plainPassword;
            }
            catch (CryptographicException)
            {
                return false;
            }
        }

        private bool HasMasterKeyLayer(byte[] encryptedPassword)
        {
            if (encryptedPassword.Length < 4) return false;

            return encryptedPassword[^4] == 0xDE &&
                   encryptedPassword[^3] == 0xAD &&
                   encryptedPassword[^2] == 0xBE &&
                   encryptedPassword[^1] == 0xEF;
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
            if (ciphertextWithTag.Length < _tagSize)
                throw new CryptographicException("Datos cifrados insuficientes");

            byte[] ciphertext = new byte[ciphertextWithTag.Length - _tagSize];
            byte[] tag = new byte[_tagSize];

            Buffer.BlockCopy(ciphertextWithTag, 0, ciphertext, 0, ciphertext.Length);
            Buffer.BlockCopy(ciphertextWithTag, ciphertext.Length, tag, 0, _tagSize);

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

        public static string DiagnoseEncryptedPassword(byte[] encryptedPassword, string masterKey)
        {
            try
            {
                Console.WriteLine($"=== DIAGNÓSTICO CONTRASEÑA CIFRADA ===");
                Console.WriteLine($"Longitud total: {encryptedPassword.Length} bytes");

                bool hasMarker = encryptedPassword.Length >= 4 &&
                                encryptedPassword[^4] == 0xDE &&
                                encryptedPassword[^3] == 0xAD &&
                                encryptedPassword[^2] == 0xBE &&
                                encryptedPassword[^1] == 0xEF;

                Console.WriteLine($"Tiene marker DEADBEEF: {hasMarker}");
                Console.WriteLine($"Formato: {(hasMarker ? "NUEVO (MasterKey)" : "ANTIGUO (PBKDF2)")}");

                return $"Diagnóstico completado - Formato: {(hasMarker ? "NUEVO" : "ANTIGUO")}";
            }
            catch (Exception ex)
            {
                return $"Error en diagnóstico: {ex.Message}";
            }
        }
    }
}
using System.Security.Cryptography;
using System.Text;

namespace NanoKVMSharp
{
    public static class CryptoUtils
    {
        // Секретный ключ (фиксированный, как в Python)
        public static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("nanokvm-sipeed-2024");

        /// <summary>
        /// Реализация OpenSSL EVP_BytesToKey для генерации ключа и IV.
        /// </summary>
        /// <param name="password">Пароль (в данном случае SecretKey)</param>
        /// <param name="salt">Соль (8 байт)</param>
        /// <param name="keyLength">Длина ключа (по умолчанию 32 байта для AES-256)</param>
        /// <param name="ivLength">Длина IV (по умолчанию 16 байт для AES)</param>
        /// <returns>Кортеж (ключ, IV)</returns>
        public static (byte[] Key, byte[] IV) DeriveKeyAndIV(byte[] password, byte[] salt, int keyLength = 32, int ivLength = 16)
        {
            if (salt.Length != 8)
                throw new ArgumentException("Salt должен быть длиной 8 байт.");
            var derived = new List<byte>();
            byte[] block = [];

            while (derived.Count < keyLength + ivLength)
            {
                // Блок = MD5(block + password + salt)
                byte[] input = ConcatBlocks(block, password, salt);
                block = MD5.HashData(input);
                derived.AddRange(block);
            }

            byte[] key = [.. derived.GetRange(0, keyLength)];
            byte[] iv = [.. derived.GetRange(keyLength, ivLength)];
            return (key, iv);
        }

        /// <summary>
        /// Шифрование данных в режиме AES-CBC с PKCS7 padding.
        /// </summary>
        /// <param name="plainText">Открытый текст</param>
        /// <param name="password">Пароль (SecretKey)</param>
        /// <returns>Зашифрованные данные в формате "Salted__{salt}{ciphertext}"</returns>
        public static byte[] EncryptAES256CBC(string plainText, byte[] password)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] salt = GenerateSalt(); // 8 байт случайной соли

            // Генерация ключа и IV
            var (key, iv) = DeriveKeyAndIV(password, salt);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor();
            byte[] cipherText = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Формат: Salted__{salt}{ciphertext}
            byte[] header = Encoding.ASCII.GetBytes("Salted__");
            return ConcatBlocks(header, salt, cipherText);
        }

        /// <summary>
        /// Обфускация пароля (шифрование + Base64 + URL-кодирование).
        /// </summary>
        /// <param name="password">Пароль в виде строки</param>
        /// <returns>Обфусцированный пароль</returns>
        public static string ObfuscatePassword(string password)
        {
            byte[] encrypted = EncryptAES256CBC(password, SecretKey);
            string base64 = Convert.ToBase64String(encrypted);
            return Uri.EscapeDataString(base64);
        }

        #region Вспомогательные методы

        private static byte[] ConcatBlocks(params byte[][] blocks)
        {
            int totalLength = 0;
            foreach (var block in blocks)
                totalLength += block.Length;

            byte[] result = new byte[totalLength];
            int offset = 0;
            foreach (var block in blocks)
            {
                Buffer.BlockCopy(block, 0, result, offset, block.Length);
                offset += block.Length;
            }
            return result;
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[8];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return salt;
        }

        #endregion
    }
}

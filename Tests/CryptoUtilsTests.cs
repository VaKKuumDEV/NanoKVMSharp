using NanoKVMSharp;
using System.Text;

namespace Tests
{
    public class CryptoUtilsTests
    {
        [Fact]
        public void DeriveKeyAndIV_ReturnsValidKeyAndIV()
        {
            byte[] password = Encoding.UTF8.GetBytes("password");
            byte[] salt = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

            var (key, iv) = CryptoUtils.DeriveKeyAndIV(password, salt);
            Assert.Equal(32, key.Length); // AES-256
            Assert.Equal(16, iv.Length);  // AES
        }

        [Fact]
        public void EncryptAES256CBC_ReturnsNonEmptyData()
        {
            string plainText = "test123";
            byte[] encrypted = CryptoUtils.EncryptAES256CBC(plainText, CryptoUtils.SecretKey);
            Assert.True(encrypted.Length > 0);
        }

        [Fact]
        public void ObfuscatePassword_ReturnsValidBase64()
        {
            string password = "test123";
            string result = CryptoUtils.ObfuscatePassword(password);
            Assert.Matches(@"^[A-Za-z0-9\-_%]+$", result); // Base64-URL формат
        }

        [Fact]
        public void DeriveKeyAndIV_WithDifferentPasswordsAndSalts_ReturnsUniqueKeys()
        {
            byte[] password1 = Encoding.UTF8.GetBytes("password1");
            byte[] password2 = Encoding.UTF8.GetBytes("password2");
            byte[] salt = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var (key1, iv1) = CryptoUtils.DeriveKeyAndIV(password1, salt);
            var (key2, iv2) = CryptoUtils.DeriveKeyAndIV(password2, salt);

            Assert.NotEqual(key1, key2);
            Assert.NotEqual(iv1, iv2);
        }

        [Fact]
        public void DeriveKeyAndIV_WithZeroSalt_ReturnsValidKeyAndIV()
        {
            byte[] password = Encoding.UTF8.GetBytes("password");
            byte[] salt = new byte[8]; // Все нули

            var (key, iv) = CryptoUtils.DeriveKeyAndIV(password, salt);
            Assert.Equal(32, key.Length);
            Assert.Equal(16, iv.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("test123")]
        [InlineData("special!@#")]
        [InlineData("long_text_1234567890_!@#")]
        public void EncryptAES256CBC_WithDifferentInputs_ReturnsNonEmptyData(string plainText)
        {
            byte[] encrypted = CryptoUtils.EncryptAES256CBC(plainText, CryptoUtils.SecretKey);
            Assert.True(encrypted.Length > 0);
        }

        [Theory]
        [InlineData("")]
        [InlineData("password")]
        [InlineData("123456")]
        [InlineData("special!@#")]
        [InlineData("long_password_1234567890_!@#")]
        public void ObfuscatePassword_WithDifferentInputs_ReturnsValidBase64(string password)
        {
            string result = CryptoUtils.ObfuscatePassword(password);
            Assert.Matches(@"^[A-Za-z0-9\-_%]+$", result); // Base64-URL формат
        }

        [Fact]
        public void ObfuscatePassword_WithCyrillicInput_ReturnsValidBase64()
        {
            string password = "пароль";
            string result = CryptoUtils.ObfuscatePassword(password);
            Assert.Matches(@"^[A-Za-z0-9\-_%]+$", result);
        }
    }
}

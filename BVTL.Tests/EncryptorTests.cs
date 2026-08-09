using Common.Common;
using Common.ICommon;
using Microsoft.VisualStudio.QualityTools.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BVTL.Tests
{
    [TestClass]
    public class EncryptorTests
    {
        private Encryptor _encryptor;

        [TestInitialize]
        public void Setup()
        {
            _encryptor = new Encryptor();
        }

        [TestMethod]
        public void HashPassword_ShouldReturnValidPBKDF2String()
        {
            // Arrange
            string rawPassword = "UserPass123!";

            // Act
            string hashedPassword = _encryptor.HashPassword(rawPassword);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(hashedPassword));
            Assert.AreEqual(48, hashedPassword.Length); // 36 bytes (16 salt + 20 hash) encoded in Base64 = 48 chars
        }

        [TestMethod]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            string rawPassword = "UserPass123!";
            string hashedPassword = _encryptor.HashPassword(rawPassword);

            // Act
            bool isValid = _encryptor.VerifyPassword(rawPassword, hashedPassword);

            // Assert
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            // Arrange
            string rawPassword = "UserPass123!";
            string wrongPassword = "WrongPassword!";
            string hashedPassword = _encryptor.HashPassword(rawPassword);

            // Act
            bool isValid = _encryptor.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void VerifyPassword_WithLegacyMD5Hash_ShouldValidateFallback()
        {
            // Arrange
            string rawPassword = "123456default";
            string legacyMD5Hash = _encryptor.MD5Hash(rawPassword); // Generates MD5 hex hash

            // Act
            bool isValid = _encryptor.VerifyPassword(rawPassword, legacyMD5Hash);

            // Assert
            Assert.IsTrue(isValid);
        }
    }
}

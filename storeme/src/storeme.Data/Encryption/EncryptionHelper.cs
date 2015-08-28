using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace storeme.Data.Encryption
{
    public static class EncryptionHelper
    {
        /// <summary>
        /// Computes the secure hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// hashSalt
        /// or
        /// password
        /// </exception>
        public static string ComputeSecureHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var passBytes = Encoding.UTF8.GetBytes(password);
            using (var hash = new Rfc2898DeriveBytes(passBytes, passBytes, (int)Math.Pow(2, 14)))
            {
                return Convert.ToBase64String(hash.GetBytes(64));
            }
        }

        /// <summary>
        /// Gets the first layer encryption key.
        /// </summary>
        /// <param name="allBytes">All bytes.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>
        /// The <see cref="AesKey" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">allBytes</exception>
        public static AesKey GenerateAesKeyFromSeed(byte[] allBytes, byte[] salt)
        {
            if (allBytes == null)
            {
                throw new ArgumentNullException(nameof(allBytes));
            }

            var hash = new Rfc2898DeriveBytes(allBytes, salt, (int)Math.Pow(2, 8));
            return new AesKey(hash.GetBytes(16), hash.GetBytes(16));
        }

        /// <summary>
        /// Generates the random aes key.
        /// </summary>
        /// <returns></returns>
        public static AesKey GenerateRandomAesKey()
        {
            using (var aes = new AesManaged())
            {
                return new AesKey(aes.Key, aes.IV);
            }
        }

        /// <summary>
        /// Encripts the in base64.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string SymmetricEncryptInBase64(string content, AesKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return Convert.ToBase64String(SymmetricEncryptData(Encoding.UTF8.GetBytes(content), key));
        }

        /// <summary>
        /// Encripts the in base64.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string SymmetricDecryptInBase64(string content, AesKey key)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentNullException("content");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            return Encoding.UTF8.GetString(SymmetricDecryptData(Convert.FromBase64String(content), key));
        }

        /// <summary>
        /// Symmetrics the decrypt data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// Decrypted data
        /// </returns>
        public static byte[] SymmetricDecryptData(byte[] data, AesKey key)
        {
            if (data == null)
            {
                throw new ArgumentException(nameof(data));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (var algorithm = new AesManaged())
            {
                return Transform(data, algorithm.CreateDecryptor(key.GetKey(), key.GetInitializationVector()));
            }
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// Encrypted data
        /// </returns>
        public static byte[] SymmetricEncryptData(byte[] data, AesKey key)
        {
            if (data == null)
            {
                throw new ArgumentException("content");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            using (var algorithm = new AesManaged())
            {
                return Transform(data, algorithm.CreateEncryptor(key.GetKey(), key.GetInitializationVector()));
            }
        }

        /// <summary>
        /// Gets the random seed.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static byte[] GetRandomSeed(int length)
        {
            var seed = new byte[length];
            RandomNumberGenerator.Create().GetBytes(seed);

            return seed;
        }

        /// <summary>
        /// Transforms the specified array.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="crypto">
        /// The crypto.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        /// </returns>
        private static byte[] Transform(byte[] array, ICryptoTransform crypto)
        {
            using (var stream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(stream, crypto, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(array, 0, array.Length);
                }

                var bytes = stream.ToArray();
                return bytes;
            }
        }
    }

}

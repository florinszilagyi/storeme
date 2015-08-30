using System;
using storeme.Data.Model;

namespace storeme.Data.Encryption
{
    /// <summary>
    /// Dashboard decryptor
    /// </summary>
    public class DashboardDecryptor
    {
        /// <summary>
        /// The key 
        /// </summary>
        private AesKey key;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardDecryptor"/> class.
        /// </summary>
        /// <param name="encryptedDashboard">The encrypted dashboard.</param>
        public DashboardDecryptor(Dashboard encryptedDashboard)
        {
            var seed = Convert.FromBase64String(encryptedDashboard.Key);
            var salt = Convert.FromBase64String(encryptedDashboard.Salt);
            this.key = EncryptionHelper.GenerateAesKeyFromSeed(seed, salt);
        }

        /// <summary>
        /// Decrypts the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="withFileContent">if set to <c>true</c> [with file content].</param>
        /// <returns></returns>
        public DashboardItem Decrypt(DashboardItem item, bool withFileContent = false)
        {
            var newItem = new DashboardItem();
            newItem.Name = EncryptionHelper.SymmetricDecryptInBase64(item.Name, this.key);
            newItem.Path = EncryptionHelper.SymmetricDecryptInBase64(item.Path, this.key);
            newItem.AddedOn = item.AddedOn;
            newItem.IsFolder = item.IsFolder;
            newItem.Id = item.Id;

            if (item.File != null && !item.IsFolder)
            {
                newItem.File = this.DecryptFile(item.File, withFileContent);
            }

            return newItem;
        }

        /// <summary>
        /// Decrypts the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="withContent">if set to <c>true</c> [with content].</param>
        /// <returns></returns>
        public DashboardFile DecryptFile(DashboardFile file, bool withContent = false)
        {
            var newFile = new DashboardFile();
            newFile.Content = withContent ? EncryptionHelper.SymmetricDecryptData(file.Content, this.key) : new byte[0];
            newFile.MediaType = EncryptionHelper.SymmetricDecryptInBase64(file.MediaType, this.key);
            newFile.Size = file.Size;

            return newFile;
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="string">The string.</param>
        /// <returns></returns>
        public string DecryptString(string @string)
        {
            return EncryptionHelper.SymmetricDecryptInBase64(@string, this.key);
        }
    }
}
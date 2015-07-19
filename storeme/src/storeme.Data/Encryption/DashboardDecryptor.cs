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
        /// The dashboard
        /// </summary>
        private readonly Dashboard dashboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardDecryptor"/> class.
        /// </summary>
        /// <param name="encryptedDashboard">The encrypted dashboard.</param>
        public DashboardDecryptor(Dashboard encryptedDashboard)
        {
            var seed = Convert.FromBase64String(encryptedDashboard.Key);
            var salt = Convert.FromBase64String(encryptedDashboard.Salt);
            this.dashboard = encryptedDashboard;
            this.key = EncryptionHelper.GenerateAesKeyFromSeed(seed, salt);
        }

        /// <summary>
        /// Decrypts the file.
        /// </summary>
        /// <returns></returns>
        public DashboardFile DecryptFile()
        {
            return new DashboardFile
            {
                Content = EncryptionHelper.SymmetricDecryptData(this.dashboard.File.Content, this.key),
                Name = EncryptionHelper.SymmetricDecryptInBase64(this.dashboard.File.Name, this.key),
                AddedOn = this.dashboard.File.AddedOn
            };
        }
    }
}
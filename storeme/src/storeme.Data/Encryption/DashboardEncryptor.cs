using System;
using System.Text;
using System.Threading.Tasks;
using storeme.Data.Model;

namespace storeme.Data.Encryption
{
    public class DashboardEncryptor
    {
        /// <summary>
        /// The seed length
        /// </summary>
        private const int SeedLength = 8;

        /// <summary>
        /// The seed
        /// </summary>
        private readonly byte[] seed;

        /// <summary>
        /// The salt
        /// </summary>
        private readonly byte[] salt;

        /// <summary>
        /// The key 
        /// </summary>
        private AesKey key;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardEncryptor"/> class.
        /// </summary>
        public DashboardEncryptor()
        {
            this.seed = EncryptionHelper.GetRandomSeed(SeedLength);

            this.salt = EncryptionHelper.GetRandomSeed(32);
            this.key = EncryptionHelper.GenerateAesKeyFromSeed(seed, salt);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardEncryptor" /> class.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        public DashboardEncryptor(Dashboard dashboard)
        {
            this.seed = Convert.FromBase64String(dashboard.Key);
            this.salt = Convert.FromBase64String(dashboard.Salt);
            this.key = EncryptionHelper.GenerateAesKeyFromSeed(seed, salt);
        }

        public string PassCode => Convert.ToBase64String(this.seed);

        public Dashboard Dashboard => new Dashboard { Key = EncryptionHelper.ComputeSecureHash(Convert.ToBase64String(this.seed)), Salt = Convert.ToBase64String(this.salt) };

        /// <summary>
        /// Gets the encrypted file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="path">The path.</param>
        /// <param name="fileData">The file data.</param>
        /// <param name="mime">The MIME.</param>
        /// <returns></returns>
        public DashboardItem CreateEncryptedItem(string fileName, string path, byte[] fileData = null, string mime = "")
        {
            path = path ?? string.Empty;
            var item = new DashboardItem();
            if (fileData != null)
            {
                var file = new DashboardFile();
                file.Content = EncryptionHelper.SymmetricEncryptData(fileData, this.key);
                file.Size = fileData.Length;
                file.MediaType = EncryptionHelper.SymmetricEncryptInBase64(mime, key);
                item.File = file;
            }

            item.Name = EncryptionHelper.SymmetricEncryptInBase64(fileName, this.key);
            item.Path = EncryptionHelper.SymmetricEncryptInBase64(path, this.key);
            item.IsFolder = fileData == null;
            return item;
        }
    }
}

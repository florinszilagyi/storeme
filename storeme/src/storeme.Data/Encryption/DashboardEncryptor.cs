using System;
using System.Text;
using System.Threading.Tasks;
using storeme.Data.Model;

namespace storeme.Data.Encryption
{
    public class DashboardEncryptor
    {
        private readonly byte[] fileData;
        private readonly string fileName;

        /// <summary>
        /// The seed length
        /// </summary>
        private const int SeedLength = 6;

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
        /// <param name="dashboard">The dashboard.</param>
        /// <param name="fileData"></param>
        /// <param name="fileName"></param>
        public DashboardEncryptor(byte[] fileData, string fileName)
        {
            this.fileData = fileData;
            this.fileName = fileName;
            this.seed = EncryptionHelper.GetRandomSeed(SeedLength);

            this.salt = EncryptionHelper.GetRandomSeed(32);
            this.key = EncryptionHelper.GenerateAesKeyFromSeed(seed, salt);
        }

        public string PassCode => Convert.ToBase64String(this.seed);

        /// <summary>
        /// Gets the encrypted file.
        /// </summary>
        /// <returns></returns>
        public Task<Dashboard> Encrypt()
        {
            return Task.Run(() =>
            {
                var file = new DashboardFile();
                file.Content = EncryptionHelper.SymmetricEncryptData(this.fileData, this.key);
                file.Name = EncryptionHelper.SymmetricEncryptInBase64(this.fileName, this.key);
                return new Dashboard
                {
                    File = file,
                    Key = Convert.ToBase64String(seed),
                    Salt = Convert.ToBase64String(this.salt)
                };
            });
        }
    }
}

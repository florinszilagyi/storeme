using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using storeme.Data.Encryption;
using storeme.Data.Model;

namespace storeme.Data
{
    public class DashboardRepository
    {
        private readonly DashboardContext context = new DashboardContext();

        /// <summary>
        /// Inserts the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <returns></returns>
        public async Task Insert(Dashboard dashboard)
        {
            await context.Dashboards.InsertOneAsync(dashboard);
        }

        /// <summary>
        /// Updates the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <returns></returns>
        public async Task Update(Dashboard dashboard)
        {
            await context.Dashboards.ReplaceOneAsync(f => f.Id == dashboard.Id, dashboard);
        }

        /// <summary>
        /// Finds the by access code.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        public async Task<Dashboard> FindByAccessCode(string accessCode)
        {
            var key = EncryptionHelper.ComputeSecureHash(accessCode);
            return await context.Dashboards.Find(f => f.Key == key).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the items by path.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<List<DashboardItem>> GetItemsByPath(string accessCode, string path)
        {
            var dashboard = await this.FindByAccessCode(accessCode);
            var decryptor = new DashboardDecryptor(dashboard);
            return dashboard.DashboardItems.Select(i => decryptor.DecryptItem(i)).Where(i => i.Path.Trim('/') == path.Trim('/')).ToList();
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<DashboardItem> GetItem(string accessCode, string path, string name)
        {
            var dashboard = await this.FindByAccessCode(accessCode);
            var decryptor = new DashboardDecryptor(dashboard);
            return dashboard.DashboardItems.Select(i => decryptor.DecryptItem(i, true)).First(i => i.Path.Trim('/') == path.Trim('/') && i.Name.Trim('/') == name.Trim('/'));
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task DeleteItem(string accessCode, string path, string name)
        {
            var dashboard = await this.FindByAccessCode(accessCode);
            var decryptor = new DashboardDecryptor(dashboard);
            var decryptedItems = dashboard.DashboardItems.Select(item => decryptor.DecryptItem(item)).ToList();
            foreach (var item in decryptedItems)
            {
                if (item.Name.Trim('/') != name.Trim('/') || item.Path.Trim('/') != path.Trim('/'))
                {
                    continue;
                }

                dashboard.DashboardItems.Remove(dashboard.DashboardItems.First(d => d.Id == item.Id));
                if (!item.IsFolder) break;

                var subItems = decryptedItems.Where(i => i.Path.Trim('/').Replace('/', '\\').StartsWith(System.IO.Path.Combine(path, name))).ToList();
                subItems.ForEach(i => dashboard.DashboardItems.Remove(dashboard.DashboardItems.First(it => it.Id == i.Id)));
            }

            await this.Update(dashboard);
        }
    }
}

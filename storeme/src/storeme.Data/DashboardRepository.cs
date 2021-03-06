﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using storeme.Data.Encryption;
using storeme.Data.Model;

namespace storeme.Data
{
    public class SqlDashboardRepository : IDashboardRepository
    {
        private DashboardContext context = new DashboardContext();

        /// <summary>
        /// Inserts the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <returns></returns>
        public async Task Insert(Dashboard dashboard)
        {

            context.Dashboards.Add(dashboard);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the specified dashboard.
        /// </summary>
        /// <param name="dashboard">The dashboard.</param>
        /// <returns></returns>
        public async Task Update(Dashboard dashboard)
        {
            context.Dashboards.Attach(dashboard);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Finds the by access code.
        /// </summary>
        /// <param name="accessCode">The access code.</param>
        /// <returns></returns>
        public async Task<Dashboard> FindByAccessCode(string accessCode)
        {
            var key = EncryptionHelper.ComputeSecureHash(accessCode);
            return await context.Dashboards.FirstOrDefaultAsync(f => f.Key == key);
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
            return dashboard.DashboardItems.Select(i => decryptor.Decrypt(i)).Where(i => i.Path.Trim('/') == path.Trim('/')).ToList();
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
            return dashboard.DashboardItems.Select(i => decryptor.Decrypt(i, true)).First(i => i.Path.Trim('/') == path.Trim('/') && i.Name.Trim('/') == name.Trim('/'));
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
            var decryptedItems = dashboard.DashboardItems.Select(item => decryptor.Decrypt(item)).ToList();
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

        /// <summary>
        /// Clears the old values.
        /// </summary>
        /// <param name="daysOld">The days old.</param>
        /// <returns></returns>
        public async Task ClearOldValues(int daysOld)
        {
            var maxDate = DateTime.Now.AddDays(-1 * daysOld);
            var toDelete = this.context.Dashboards.Where(d => d.Created <= maxDate);
            this.context.Dashboards.RemoveRange(toDelete);

            await this.context.SaveChangesAsync();
        }

        public async Task<List<Tuple<string, int, int>>> GetDashboardsByDate()
        {
            var _30daysago = DateTime.Now.AddDays(-30);
            var last30Days = context.Dashboards.Where(d => d.Created >= _30daysago);
            var grouped = await last30Days.GroupBy(s => s.Created).ToListAsync();
            return grouped.Select(d => new Tuple<string, int, int>(d.Key.ToShortDateString(), d.Count(), d.Sum(di => di.DashboardItems.Count))).ToList();
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}

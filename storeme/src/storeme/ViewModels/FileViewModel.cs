using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using storeme.Data.Model;

namespace storeme.ViewModels
{
    /// <summary>
    /// File view model
    /// </summary>
    public class FileViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the rights.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        public Rights Rights { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public FileType Type { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Froms the dashboard item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="fullRights">if set to <c>true</c> [full rights].</param>
        /// <returns></returns>
        public static FileViewModel FromDashboardItem(DashboardItem item, bool fullRights = false)
        {
            return new FileViewModel
            {
                Date = item.AddedOn,
                Name = item.Name,
                Rights = fullRights ? Rights.Read | Rights.Write : Rights.Read,
                Size = item.File?.Size ?? 0,
                Type = item.IsFolder ? FileType.Dir : FileType.File
            };

        }
    }
}
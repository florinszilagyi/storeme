using System;

namespace storeme.Data.Model
{
    public class DashboardFile
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the added on.
        /// </summary>
        /// <value>
        /// The added on.
        /// </value>
        public DateTime AddedOn { get; set; } = DateTime.UtcNow.Date;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public byte[] Content { get; set; }
    }
}
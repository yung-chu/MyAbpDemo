using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    public class DataSyncInput
    {
        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        public Guid TaskId { get; set; }


        /// <summary>
        /// 同步数据json
        /// </summary>
        public string SyncData { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime { get; set; }
    }
}

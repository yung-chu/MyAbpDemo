using System;
using System.Collections.Generic;
using System.Text;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    /// <summary>
    /// 后台任务参数
    /// </summary>
    public class ApiDataSyncJobArgs: DataSyncInput
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiDataSyncJobArgs()
        {
            base.TaskId = Guid.NewGuid();
            base.CreationTime = DateTime.Now;
        }

        /// <summary>
        /// 目标公园Id
        /// </summary>
        public int TargetParkId { get; set; }
    }
}

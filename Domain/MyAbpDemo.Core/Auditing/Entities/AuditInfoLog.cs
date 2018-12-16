using System;
using System.Collections.Generic;
using System.Text;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace MyAbpDemo.Core
{
    public class AuditInfoLog : Entity<long>, IMayHaveTenant
    {
        /// <summary>
        /// TenantId.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// ImpersonatorUserId.
        /// </summary>
        public long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// ImpersonatorTenantId.
        /// </summary>
        public int? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// Service (class/interface) name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Executed method name.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Calling parameters.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Start time of the method execution.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Total duration of the method call.
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        public string ClientIpAddress { get; set; }

        /// <summary>
        /// Name (generally computer name) of the client.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        public string BrowserInfo { get; set; }

        /// <summary>
        /// Optional custom data that can be filled and used.
        /// </summary>
        public string CustomData { get; set; }


        public string Exception { get; set; }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditInfo"></param>
        /// <returns></returns>
        public static AuditInfoLog Create(AuditInfo auditInfo)
        {
            var exceptionOrSuccessMessage = auditInfo.Exception?.Message;
     
            return new AuditInfoLog
            {
                TenantId = auditInfo.TenantId,
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(50),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(50),
                Parameters = auditInfo.Parameters.TruncateWithPostfix(50),
                ExecutionTime = auditInfo.ExecutionTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(50),
                ClientName = auditInfo.ClientName.TruncateWithPostfix(50),
                BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(50),
                Exception = exceptionOrSuccessMessage,
                ImpersonatorUserId = auditInfo.ImpersonatorUserId,
                ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
                CustomData = auditInfo.CustomData.TruncateWithPostfix(50)
            };
        }
    }
}

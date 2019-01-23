using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Repositories;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    /// <summary>
    /// 参考源码 Abp.Zero.Common Auditing/AuditingStore
    /// </summary>
    public class MyAuditingStore : IAuditingStore,ITransientDependency
    {
        private readonly IAuditingRepository _auditingRepository;
        public MyAuditingStore(IAuditingRepository auditingRepository)
        {
            _auditingRepository = auditingRepository;
        }

        public  Task SaveAsync(AuditInfo auditInfo)
        {
            return  _auditingRepository.InsertAsync(AuditInfoLog.Create(auditInfo));
        }
    }
}

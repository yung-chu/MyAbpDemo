using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class AuditingRepository :MyAbpDemoRepositoryBase<AuditInfoLog, long>, IAuditingRepository
    {
        public AuditingRepository(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}

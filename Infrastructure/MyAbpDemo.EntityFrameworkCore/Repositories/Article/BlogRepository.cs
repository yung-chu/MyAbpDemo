using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class BlogRepository:MyAbpDemoRepositoryBase<Blog>,IBlogRepository
    {
        public BlogRepository(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}

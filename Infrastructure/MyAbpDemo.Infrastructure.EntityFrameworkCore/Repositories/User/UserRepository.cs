using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class UserRepository: MyAbpDemoRepositoryBase<User, long>,IUserRepository
    {
        public UserRepository(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}

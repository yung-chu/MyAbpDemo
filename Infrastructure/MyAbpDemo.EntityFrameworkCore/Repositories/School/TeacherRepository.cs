using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.EFCore
{
    public class TeacherRepository: MyAbpDemoRepositoryBase<Teacher,long>,ITeacherRepository
    {
        public TeacherRepository(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}

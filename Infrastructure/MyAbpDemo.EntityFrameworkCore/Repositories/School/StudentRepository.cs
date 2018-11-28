using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.EFCore
{
    public class StudentRepository: MyAbpDemoRepositoryBase<Student, long>, IStudentRepository
    {
        public StudentRepository(IDbContextProvider<MyAbpDemoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}

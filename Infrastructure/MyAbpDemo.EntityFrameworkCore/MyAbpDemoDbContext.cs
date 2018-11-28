using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.EFCore
{
    public class MyAbpDemoDbContext : AbpDbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        //https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/configuring-dbcontext
        //集成EfCore
        //https://github.com/ABPFrameWorkGroup/AbpDocument2Chinese/blob/master/Markdown/Abp/9.3ABP%E5%9F%BA%E7%A1%80%E8%AE%BE%E6%96%BD%E5%B1%82-%E9%9B%86%E6%88%90EntityFrameworkCore.md
        public MyAbpDemoDbContext(DbContextOptions<MyAbpDemoDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //teacher 
            modelBuilder.Entity<Teacher>().HasData(new Teacher()
            {
                Id = 1,
                Name = "朱老师",
                Age = 18,
                IsActive = true,
                IsReview = true

            });

            //student
            modelBuilder.Entity<Student>().HasData(new Student()
            {
                Id = 1,
                Name = "学生1",
                Age = 18,
                IsActive = true,
                TeacherId = 1
            });

            modelBuilder.Entity<Student>().HasData(new Student()
            {
                Id=2,
                Name = "学生2",
                Age = 36,
                IsActive = true,
                TeacherId = 1
            });
        }
    }
}

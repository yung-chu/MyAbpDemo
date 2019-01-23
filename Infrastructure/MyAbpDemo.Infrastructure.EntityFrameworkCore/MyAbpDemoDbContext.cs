using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class MyAbpDemoDbContext : AbpDbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }


        //集成EfCore
        //https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/configuring-dbcontext
        //https://github.com/ABPFrameWorkGroup/AbpDocument2Chinese/blob/master/Markdown/Abp/9.3ABP%E5%9F%BA%E7%A1%80%E8%AE%BE%E6%96%BD%E5%B1%82-%E9%9B%86%E6%88%90EntityFrameworkCore.md
        public MyAbpDemoDbContext(DbContextOptions<MyAbpDemoDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //https://www.learnentityframeworkcore.com/configuration/fluent-api
            //fluent api
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null);

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }

            //初始化数据
            modelBuilder.Seed();
        }
    }
}

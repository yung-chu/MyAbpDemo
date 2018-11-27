using System;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.EFCore
{
    [DependsOn(typeof(CoreModule),typeof(AbpEntityFrameworkCoreModule))]
    public class EfCoreModule : AbpModule
    {
        /// <summary>
        /// 配置DbContext
        /// </summary>
        public override void PreInitialize()
        {
            //我们使用给定的连接字符串并使用Sql Server作为数据库提供器。
            //通常 options.ConnectionString 的值就是 default连接字符串。
            //但是ABP使用 IConnectionStringResolver 来确定。
            //所以，这个行为方式是可以改变的并且连接字符串可以动态的切换。
            //每当DbContext被实例化的时候，这个动作会传递给 AddAbpDbContext 方法。
            //所以，你有机会可以返还不同条件下的连接字符串。
            //Configuration.DefaultNameOrConnectionString

            Configuration.Modules.AbpEfCore().AddDbContext<MyAbpDemoDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    options.DbContextOptions.UseSqlServer(options.ExistingConnection);
                }
                else
                {
                    options.DbContextOptions.UseSqlServer(options.ConnectionString);
                }
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EfCoreModule).GetAssembly());
        }
    }
}

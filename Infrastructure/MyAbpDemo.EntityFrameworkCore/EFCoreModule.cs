using System;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    [DependsOn(typeof(CoreModule),typeof(AbpEntityFrameworkCoreModule))]
    public class EfCoreModule : AbpModule
    {
        /// <summary>
        /// 配置DbContext
        /// </summary>
        public override void PreInitialize()
        {
            //每当DbContext被实例化的时候，这个动作会传递给 AddAbpDbContext 方法。
            //所以，你有机会可以返还不同条件下的连接字符串。
            //Configuration.DefaultNameOrConnectionString
            // //https://github.com/ABPFrameWorkGroup/AbpDocument2Chinese/blob/master/Markdown/Abp/9.3ABP%E5%9F%BA%E7%A1%80%E8%AE%BE%E6%96%BD%E5%B1%82-%E9%9B%86%E6%88%90EntityFrameworkCore.md
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

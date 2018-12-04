
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace MyAbpDemo.Infrastructure.EFCore
{
    /// <summary>
    /// 设计时 DbContext 创建
    /// https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/cli/dbcontext-creation
    /// </summary>
    public class MyAbpDemoDbContextFactory: IDesignTimeDbContextFactory<MyAbpDemoDbContext>
    {
        public  MyAbpDemoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyAbpDemoDbContext>();
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=MyAbpDemo;User ID=sa;Password=a135246A");

            return new MyAbpDemoDbContext(optionsBuilder.Options);
        }
    }
}

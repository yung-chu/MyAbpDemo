using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Castle.Logging.NLog;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyAbpDemo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //NETCore下IConfiguration和IOptions的用法
        //https://www.jianshu.com/p/b9416867e6e6
        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            //配置Abp和依赖注入，在最后调用
            return services.AddAbp<ApiModule>(
                // Configure Log4Net logging
                //options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                //    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                //)

                // Configure Nlog Logging
                //https://www.cnblogs.com/moyhui/p/9358164.html
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpNLog().WithConfig("NLog.config")
                )
            );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {  
            
            //初始化ABP框架和所有其他模块，这个应该首先被调用
            app.UseAbp();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

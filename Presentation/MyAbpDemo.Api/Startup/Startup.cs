using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Castle.Logging.NLog;
using Castle.Facilities.Logging;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAbpDemo.Infrastructure;
using MyAbpDemo.Infrastructure.Api;
using MyAbpDemo.Infrastructure.Api.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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
            //获取配置
            services.Configure<JwtSetting>(Configuration.GetSection("JWT"));


            //添加筛选器
            //https://docs.microsoft.com/zh-cn/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.1#action-filters
            services.AddMvc(option =>
                {
                    option.Filters.Add(typeof(MyActionFilter));
                    option.Filters.Add(typeof(MyAbpAuditActionFilter));
                    option.Filters.Add(typeof(MyAbpExceptionFilter));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            //注册swagger
            // https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "zhuyong",
                        Email = string.Empty,
                        Url = "www.baidu.com"
                    }
               });

                //显示注释
                foreach (var file in Directory.GetFiles(AppContext.BaseDirectory, "MyAbpDemo.*.xml"))
                {
                    options.IncludeXmlComments(file);
                }

                //添加jwt
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example - Authorization: Bearer {token}",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });

            });

            //模型验证自定义结果输出
            //https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.1
            //https://www.strathweb.com/2018/02/exploring-the-apicontrollerattribute-and-its-features-for-asp-net-core-mvc-2-1/
            services.Configure<ApiBehaviorOptions>(options =>
              options.InvalidModelStateResponseFactory = InvalidModelStateExecutor.Executer
            );

            //注册的 CORS 服务
            services.AddCors(builder =>
            {
                var host = Configuration.GetValue<string>("AllowedHosts")?.Split(";");
                builder.AddDefaultPolicy(p =>
                {
                    p.WithOrigins(host)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
             });


            #region JwtBearer认证
            //https://www.cnblogs.com/weipengpeng/p/9651336.html

            services.AddAuthorization(options =>
            {
                //var authRequirement = new AuthRequirement("/api/user/login", ClaimTypes.Name, Configuration["JWT:Issuer"], Configuration["JWT:Audience"], signingCredentials);
                //options.AddPolicy("POS", policy => policy.Requirements.Add(authRequirement));
            }).AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"])),
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = JwtBearerEventHandler.OnTokenValidated
                };
            });

            #endregion


            //ABP集成到ASP.NET Core和依赖注入，在最后调用
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

            app.UseAuthentication();//添加jwt身份认证(app.UseMvc()之前调用)
            app.UseCors();//跨域
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }
    }
}

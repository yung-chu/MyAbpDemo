using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace MyAbpDemo.Api.Swagger
{
    public static class RegisterSwaggerGen
    {
        /// <summary>
        /// 添加Swagger
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerGen(this IServiceCollection services)
        {
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

                //注册上传文件Filter
                options.OperationFilter<FileUploadFilter>();
            });
        }
    }
}

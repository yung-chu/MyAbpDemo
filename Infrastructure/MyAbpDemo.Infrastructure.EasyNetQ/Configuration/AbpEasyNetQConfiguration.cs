using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    public class AbpEasyNetQConfiguration : IAbpEasyNetQConfiguration
    {
        /// <summary>
        /// the connection string for easyNetQ connect to RabbitMq
        /// eg:"host=myServer;virtualHost=myVirtualHost;username=mike;password=topsecret"
        /// for more infomation ,please visit https://github.com/EasyNetQ/EasyNetQ/wiki/Introduction.
        /// </summary>
        public string RabbitMqConnectionString { get; set; }
    }
}

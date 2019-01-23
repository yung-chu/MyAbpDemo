using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.Application
{
    public class MessageAppService:AppServiceBase, IMessageAppService
    {
        public void SendMessage(string msg)
        {
            Logger.Info($"执行了hangfirejob消息:{msg}" + DateTime.Now.ToLocalTime());
        }
    }
}

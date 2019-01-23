using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;

namespace MyAbpDemo.Application
{
    public interface IMessageAppService:IApplicationService
    {
        void SendMessage(string msg);
    }
}

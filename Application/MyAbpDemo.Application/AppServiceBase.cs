using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;
using Abp.Auditing;

namespace MyAbpDemo.Application
{
    [Audited]
    public abstract class AppServiceBase: ApplicationService
    {

    }
}

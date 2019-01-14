using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application
{
    public interface ITeacherAppService: IApplicationService
    {
        Task<Result<List<GetTeacherListOutput>>> GetTeacherListAsync();
    }
}

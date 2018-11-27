using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    public interface ITeacherAppService: IApplicationService
    {
        Task<List<GetTeacherListOutput>> GetTeacherList();
    }
}

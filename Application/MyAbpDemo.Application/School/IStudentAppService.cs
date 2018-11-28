using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    public interface IStudentAppService
    {
        Task<List<GetStudentListOutput>> GetStudentList();
    }
}

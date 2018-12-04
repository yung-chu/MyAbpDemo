using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace MyAbpDemo.Core
{
    /// <summary>
    /// 领域服务提供给应用服务层使用
    /// 领域服务返回领域对象(实体或者值类型)
    /// 
    /// https://github.com/ABPFrameWorkGroup/AbpDocument2Chinese/blob/master/Markdown/Abp/3.4ABP%E9%A2%86%E5%9F%9F%E5%B1%82-%E9%A2%86%E5%9F%9F%E6%9C%8D%E5%8A%A1.md
    /// </summary>
    public interface IStudentDomainService: IDomainService
    {
        Task CreateStudent(Student student);
    }
}

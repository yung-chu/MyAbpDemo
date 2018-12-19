using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using MyAbpDemo.Infrastructure;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    public interface IUserAppService : IApplicationService
    {
        Task<Result<GetUserListOutput>> GetUserList();

        Task<Result<LoginOutPut>> GetUser(LoginInput input);
    }
}

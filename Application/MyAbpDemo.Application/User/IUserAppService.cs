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
        Task<Result<GetUserListOutput>> GetUserListAsync();

        Task<Result<LoginOutPut>> GetUserAsync(LoginInput input);

        Task<Result> UpdateUserAsync(UpdateUserInput input);
    }
}

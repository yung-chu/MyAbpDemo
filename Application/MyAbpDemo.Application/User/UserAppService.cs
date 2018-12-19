using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application
{
    public class UserAppService: AppServiceBase,IUserAppService
    {
        private readonly IUserRepository _userRepository;
        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// GetUserList
        /// </summary>
        /// <returns></returns>
        public async Task<Result<GetUserListOutput>> GetUserList()
        {
            var model = new GetUserListOutput
            {
                Users = await _userRepository.GetAll().ProjectTo<UserDto>().ToListAsync()
            };

            return Result.FromData(model);
        }

        /// <summary>
        /// GetUser login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result<LoginOutPut>> GetUser(LoginInput input)
        {
            var result = Result.FromData(new LoginOutPut());
            var countAsync =await _userRepository.GetAll().CountAsync(a=>a.UserName== input.UserName&&a.Password==input.Password);
            if (countAsync == 0)
            {
                result.Code = ResultCode.ParameterFailed;
            }

            return result;
        }
    }
}

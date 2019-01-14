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
        public async Task<Result<GetUserListOutput>> GetUserListAsync()
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
        public async Task<Result<LoginOutPut>> GetUserAsync(LoginInput input)
        {
            var result = Result.FromData(new LoginOutPut());
            var isExist =await _userRepository.GetAll().AnyAsync(a=>a.UserName== input.UserName&&a.Password==input.Password);
            if (!isExist)
            {
                result.Code = ResultCode.ParameterFailed;
            }

            return result;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> UpdateUserAsync(UpdateUserInput input)
        {
            var user = await _userRepository.GetAll().FirstAsync(a => a.Id == input.Id);
            user.UserName = input.UserName;
            await _userRepository.UpdateAsync(user);
            return  Result.Ok();
        }
    }
}

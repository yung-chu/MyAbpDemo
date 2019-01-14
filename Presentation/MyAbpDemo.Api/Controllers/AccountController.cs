using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAbpDemo.Application;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;
using MyAbpDemo.Infrastructure.Api;

namespace MyAbpDemo.Api.Controllers
{
    [Authorize]
    public class AccountController : ApiControllerBase
    {
        private IUserAppService _userAppService;
        private readonly JwtSetting _jwtSetting;
        private readonly ICacheManager _cacheManager;
     
        public AccountController(IUserAppService userAppService, IOptions<JwtSetting> jwtSetting, ICacheManager cacheManager)
        {
            _userAppService = userAppService;
            _jwtSetting = jwtSetting.Value;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("users")]
        [ProducesResponseType(typeof(GetUserListOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Users()
        {
            var result =await _userAppService.GetUserListAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }


        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPatch("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserInput input)
        {
         
            var result = await _userAppService.UpdateUserAsync(input);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginInput input)
        {
            var result = await _userAppService.GetUserAsync(input);
            if (result.IsSuccess)
            {
                //生成用户token
                result.Data.Token = JwtToken.GetToken(_jwtSetting, input.UserName, "45465@qq.com");
                result.Data.UserName = input.UserName;

                //缓存token
                _cacheManager.SetUserTokenCache(input.UserName, result.Data.Token);

                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }
    }
}
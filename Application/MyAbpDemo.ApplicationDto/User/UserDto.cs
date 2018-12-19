using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    ///返回用户
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Emial { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}

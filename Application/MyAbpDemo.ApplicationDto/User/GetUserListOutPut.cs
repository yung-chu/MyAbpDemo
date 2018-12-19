using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    ///  用户列表
    /// </summary>
    public class GetUserListOutput
    {
        /// <summary>
        /// 用户
        /// </summary>
        public List<UserDto> Users { get; set; }
    }
}

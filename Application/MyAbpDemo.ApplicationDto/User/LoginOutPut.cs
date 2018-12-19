using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    public class LoginOutPut
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string Token { get; set; }
    }
}

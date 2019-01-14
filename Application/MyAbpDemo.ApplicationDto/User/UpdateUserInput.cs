using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    public class UpdateUserInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Required]
        public int Id { get; set; }


        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }
    }
}

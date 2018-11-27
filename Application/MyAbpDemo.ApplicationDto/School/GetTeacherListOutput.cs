using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    public class GetTeacherListOutput
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年纪
        /// </summary>
        public int Age { get; set; }
    }
}

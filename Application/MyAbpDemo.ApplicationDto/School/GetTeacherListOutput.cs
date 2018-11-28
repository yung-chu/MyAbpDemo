using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// 老师信息
    /// </summary>
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

        /// <summary>
        /// 学生名
        /// </summary>
        public string StudentNames { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    public class GetStudentListOutput
    {
        /// <summary>
        /// 学生名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学生年纪
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 学生是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 学生评级
        /// </summary>
        public string LearnLevel { get;  set; } 

        /// <summary>
        /// 老师名字
        /// </summary>
        public  string TeacherName { get; set; }
    }
}

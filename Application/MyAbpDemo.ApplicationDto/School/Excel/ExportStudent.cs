using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{    
    /// <summary>
    /// excel导出
    /// DisplayName为Excel标题列
    /// </summary>
    public class ExportStudent
    {
        /// <summary>
        /// 学生名字
        /// </summary>
        [DisplayName("学生名字")]
        public string Name { get; set; }

        /// <summary>
        /// 学生年纪
        /// </summary>
        [DisplayName("学生年纪")]
        public int Age { get; set; }

        /// <summary>
        /// 学生是否启用
        /// </summary>
        [DisplayName("启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 学生评级
        /// </summary>
        [DisplayName("学生评级")]
        public string LearnLevel { get; set; }

        /// <summary>
        /// 老师名字
        /// </summary>
        [DisplayName("老师名字")]
        public string TeacherName { get; set; }
    }

    /// <summary>
    /// excel 导出错误信息
    /// DisplayName为Excel标题列
    /// </summary>
    public class ExportStudentWithError: ExportStudent
    {
        [DisplayName("错误信息")]
        public string ErrorMessage { get; set; }
    }
}

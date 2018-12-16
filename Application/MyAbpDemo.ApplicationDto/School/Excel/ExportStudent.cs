using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Abp.AutoMapper;
using AutoMapper;

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
    [AutoMapFrom(typeof(ImportStudent))]
    public class ExportWithError
    {
        [DisplayName("名字")]
        public string Name { get; set; }

        [DisplayName("年纪")]
        public int Age { get; set; }

        [DisplayName("等级")]
        public string LearnLevel { get; set; }

        [DisplayName("老师编号")]
        public int TeacherId { get; set; }

        [DisplayName("错误信息")]
        public string ErrorMessage { get; set; }
    }



}

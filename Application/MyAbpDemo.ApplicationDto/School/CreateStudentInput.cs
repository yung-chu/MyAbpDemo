using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.AutoMapper;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// 创建学生
    /// </summary>
    [AutoMapTo(typeof(Student))]    // CreateStudentInput mapTo Student
    public class CreateStudentInput
    {
       /// <summary>
       /// 名字
       /// </summary>
        [Required]
        public string Name { get; set; }

       /// <summary>
       /// 年纪
       /// </summary>
        [Range(1,99)]
        public int Age { get; set; }

        /// <summary>
        /// 学生级别
        /// </summary>
        [Required]
        public LearnLevel LearnLevel { get; set; }

        /// <summary>
        /// 老师编号
        /// </summary>
        [Required]
        public int TeacherId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// excel导入
    /// DisplayName为Excel标题列
    /// </summary>
    public class ImportStudent
    {
        [DisplayName("名字")]
        public string Name { get;set;}

        [DisplayName("年纪")]
        public int Age { get; set; }

        [DisplayName("等级")]
        public string LearnLevel { get; set; }

        [DisplayName("老师编号")]
        public int TeacherId { get; set; }
    }
}

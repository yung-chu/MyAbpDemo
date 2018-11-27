using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace MyAbpDemo.Core
{
    public class Teacher : FullAuditedEntity<long>, IPassivable
    {
        public const int NameLength = 50;

        /// <summary>
        /// 名字
        /// </summary>
        [StringLength(NameLength)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 年纪
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否复合过
        /// </summary>
        public bool IsReview { get; set; }


        public  List<Student> Students { get; set; }
    }
}

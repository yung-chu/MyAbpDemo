using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace MyAbpDemo.Core
{
    public class Student: FullAuditedEntity<long>, IPassivable
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
        /// 评级
        /// </summary>
        public LearnLevel LearnLevel { get; protected set; } = LearnLevel.Standard;

        /// <summary>
        /// 更改评级内部方法
        /// </summary>
        /// <param name="learnLevel"></param>
        internal void ChangeLearnLevel(LearnLevel learnLevel)
        {
            this.LearnLevel = learnLevel;
        }

        public long TeacherId { get; set; }

        public Teacher Teacher { get; set; }
    }
}

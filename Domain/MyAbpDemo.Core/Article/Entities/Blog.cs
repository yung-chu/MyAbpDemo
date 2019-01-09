using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;

namespace MyAbpDemo.Core
{
    public class Blog: Entity
    {
        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Post> Posts { get; set; }
    }
}

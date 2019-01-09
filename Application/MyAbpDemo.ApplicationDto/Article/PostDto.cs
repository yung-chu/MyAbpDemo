using System;
using System.Collections.Generic;
using System.Text;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(Post))]
    public class PostDto:Entity
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public string Content { get; set; }
    }
}

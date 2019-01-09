using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyAbpDemo.Core
{
    public class Post: Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public string BlogUrl { get; set; }
        public Blog Blog { get; set; }
    }
}

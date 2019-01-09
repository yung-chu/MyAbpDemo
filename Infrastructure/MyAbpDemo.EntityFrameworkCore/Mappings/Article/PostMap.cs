using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            //https://docs.microsoft.com/zh-cn/ef/core/modeling/alternate-keys
            //使用备选键 非主键url作为post的外键 blogUrl
            builder.HasOne(a => a.Blog).WithMany(a => a.Posts).HasForeignKey(a => a.BlogUrl)
                .HasPrincipalKey(a => a.Url).OnDelete(DeleteBehavior.Restrict);
        }

    }
}

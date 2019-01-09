using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class AuditInfoLogMap : IEntityTypeConfiguration<AuditInfoLog>
    {
        public void Configure(EntityTypeBuilder<AuditInfoLog> builder)
        {
            builder.HasKey(o => o.Id);
        }
    }
}

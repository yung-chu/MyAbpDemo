using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    //https://stackoverflow.com/questions/26957519/ef-core-mapping-entitytypeconfiguration

    public class StudentMap : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasOne(a => a.Teacher).WithMany(a=>a.Students).HasForeignKey(a => a.TeacherId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

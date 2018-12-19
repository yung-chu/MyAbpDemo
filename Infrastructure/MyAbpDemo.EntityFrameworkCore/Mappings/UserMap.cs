using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            //拥有的实体类型
            //https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities
            //需要添加将拥有的类型存储在单独的表中  sa.ToTable("Adress") 
            //否则报错 The entity of type 'Address' is sharing the table 'User' with entities of type 'User', but there is no entity of this type with the same key value that has been marked as 'Added'.Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the key values.
            builder.OwnsOne(
                o => o.Address,
                sa =>
                {
                    sa.Property(p => p.CityId).HasColumnName("CityId");
                    sa.Property(p => p.Street).HasColumnName("Street");
                    sa.Property(p => p.Number).HasColumnName("Number");
                    sa.ToTable("Adress");
                });
        }
    }
}

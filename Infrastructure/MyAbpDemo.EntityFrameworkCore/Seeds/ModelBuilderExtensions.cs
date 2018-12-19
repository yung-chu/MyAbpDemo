using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.Core;

namespace MyAbpDemo.Infrastructure.EFCore
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //teacher 
            modelBuilder.Entity<Teacher>().HasData(new Teacher()
            {
                Id = 1,
                Name = "朱老师",
                Age = 18,
                IsActive = true,
                IsReview = true
            });

            //student
            modelBuilder.Entity<Student>().HasData(new Student()
            {
                Id = 1,
                Name = "学生1",
                Age = 18,
                IsActive = true,
                TeacherId = 1,
                LearnLevel= LearnLevel.BelowStandard
            });

            modelBuilder.Entity<Student>().HasData(new Student()
            {
                Id = 2,
                Name = "学生2",
                Age = 36,
                IsActive = true,
                TeacherId = 1,
                LearnLevel = LearnLevel.Excellent
            });

            //https://github.com/aspnet/EntityFrameworkCore/issues/12004
            modelBuilder.Entity<User>(b =>
            {
                b.HasData(new
                {
                    Id = (long?)1,
                    UserName = "test1",
                    Nickname = "小名test1",
                    Emial = "jianlive@sina.com",
                    Password = "123",
                    CreationTime=DateTime.Now,
                    IsActive=true,
                    IsDeleted=false
                });

                b.OwnsOne(e => e.Address).HasData(new
                {
                    UserId=1L,
                    CityId = 123,
                    Number = 235,
                    Street = "天顶街道"
                });
            });
        }
    }
}

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
            #region school
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
                LearnLevel = LearnLevel.BelowStandard
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

            #endregion


            //user
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



            #region article
            modelBuilder.Entity<Blog>().HasData(new Blog
            {
                Id = 1,
                Url = "www.baidu.com"
            });


            var post = new List<Post>
            {
                new Post
                {
                    Id = 1,
                    Title = "下班了",
                    Content = "6.15准时下班",
                    BlogUrl = "www.baidu.com",
                },
                new Post
                {
                    Id = 1,
                    Title = "放假了",
                    Content = "2.2放假了",
                    BlogUrl = "www.baidu.com",
                }
            };


           var posts=new List<Post>
           {
               new Post
               {
                   Id = 1,
                   Title = "下班了",
                   Content = "6.15准时下班",
                   BlogUrl = "www.baidu.com",
               }, new Post
               {
                   Id = 2,
                   Title = "过年了",
                   Content = "2.5过年了",
                   BlogUrl = "www.baidu.com",
               }
           }.ToArray();

            modelBuilder.Entity<Post>().HasData(posts);

            #endregion

        }
    }
}

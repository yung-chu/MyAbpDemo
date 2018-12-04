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
        }
    }
}

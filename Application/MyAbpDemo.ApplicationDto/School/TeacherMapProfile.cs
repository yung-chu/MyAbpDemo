using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Collections.Extensions;
using AutoMapper;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    public class TeacherMapProfile: Profile
    {
        public TeacherMapProfile()
        {
            CreateMap<Teacher, GetTeacherListOutput>().ForMember(dest => dest.StudentNames,
                opt => opt.MapFrom(a => a.Students.Select(b => b.Name).JoinAsString(",")));
        }
    }
}

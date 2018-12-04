using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.ApplicationDto
{
    public class StudentMapProfile: Profile
    {
        public StudentMapProfile()
        {
            CreateMap<ImportStudent, Student>()
                .ForMember(dest => dest.LearnLevel, opt => opt.ResolveUsing<CustomResolver>());
        }

        /// <summary>
        /// 自定义解析器（Custom value resolvers）
        /// https://www.cnblogs.com/youring2/p/automapper.html
        /// </summary>
        public class CustomResolver : IValueResolver<ImportStudent, Student, LearnLevel>
        {
            public LearnLevel Resolve(ImportStudent source, Student destination, LearnLevel destMember, ResolutionContext context)
            {
                LearnLevel learnLevel = (LearnLevel) new LearnLevel().GetEnumInfoList()
                    .First(a => a.DisplayName == source.LearnLevel).Value;

                return learnLevel;
            }
        }
    }
}

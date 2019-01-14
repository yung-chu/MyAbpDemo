using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application.School
{
    public class TeacherAppService: AppServiceBase,ITeacherAppService
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherAppService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<Result<List<GetTeacherListOutput>>> GetTeacherListAsync()
        {
            var result= await _teacherRepository.GetAll().ProjectTo<GetTeacherListOutput>().ToListAsync();
            return Result.FromData(result);
        }
    }
}

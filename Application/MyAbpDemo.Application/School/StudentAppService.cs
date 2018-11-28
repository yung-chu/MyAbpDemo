using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    public class StudentAppService:AppServiceBase,IStudentAppService
    {
        private readonly IStudentRepository _studentRepository;
        public StudentAppService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<GetStudentListOutput>> GetStudentList()
        {
            var result =await _studentRepository.GetAll().ProjectTo<GetStudentListOutput>().ToListAsync();
            return result;
        }
    }
}

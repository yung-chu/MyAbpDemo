using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace MyAbpDemo.Core
{
    public class StudentDomainService: DomainService,IStudentDomainService
    {
    
        private readonly IStudentRepository _studentRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentRepository"></param>
        public StudentDomainService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task CreateStudent(Student student)
        {
           await _studentRepository.InsertAsync(student);
        }
    }
}

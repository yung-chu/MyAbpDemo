using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Services;
using Abp.UI;

namespace MyAbpDemo.Core
{
    /// <summary>
    /// 领域服务一般用于联合多个的实体领域实体来处理业务
    /// </summary>
    public class StudentDomainService: DomainService,IStudentDomainService
    {
    
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentRepository"></param>
        public StudentDomainService(IStudentRepository studentRepository, ITeacherRepository teacherRepository)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task CreateStudent(Student student)
        {
            int count =await _teacherRepository.CountAsync(a=>a.Id==student.TeacherId);
            if (count==0)
            {
                student.TeacherId = _teacherRepository.InsertAndGetId(new Teacher
                {
                    Name = "缺省老师",
                    Age = 18
                });
            }

            var existName = _studentRepository.GetAll()
                .Any(a => string.Equals(a.Name, student.Name, StringComparison.OrdinalIgnoreCase));

            if (existName)
            {
                throw new UserFriendlyException("学生名重复");
            }

            await _studentRepository.InsertAsync(student);
        }
    }
}

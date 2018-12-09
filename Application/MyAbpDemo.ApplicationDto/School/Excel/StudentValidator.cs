using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// https://fluentvalidation.net/built-in-validators
    /// 新版本可以可以取到displayName
    /// </summary>
    public class StudentValidator: AbstractValidator<ImportStudent>
    {
        public StudentValidator(Func<string,bool> checkLearnLevel, Func<int, bool> checkTeacherId)
        {
            RuleFor(a => a.Name)
                .NotEmpty()
                .MaximumLength(Student.NameLength).WithMessage("学生名字超过最大长度{MaxLength}");

            RuleFor(a => a.Age)
                .NotEqual(0);    

            RuleFor(a => a.LearnLevel)
                .NotEmpty()
                .Must(checkLearnLevel).WithMessage("系统不存在该学生等级");

            RuleFor(a => a.TeacherId)
                .NotEqual(0)
                .Must(checkTeacherId).WithMessage("系统不存在该老师编号");

        }
    }
}

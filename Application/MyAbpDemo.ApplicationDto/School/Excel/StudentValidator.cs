using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using MyAbpDemo.Core;

namespace MyAbpDemo.ApplicationDto
{
    /// <summary>
    /// https://fluentvalidation.net/built-in-validators
    /// </summary>
    public class StudentValidator: AbstractValidator<ImportStudent>
    {
        public StudentValidator(Func<string,bool> checkLearnLevel, Func<int, bool> checkTeacherId)
        {
            RuleFor(a => a.Name)
                .NotEmpty().WithMessage("学生名字不能为空")
                .MaximumLength(Student.NameLength).WithMessage("学生名字{MaxLength}");

            RuleFor(a => a.Age)
                .NotEqual(0).WithMessage("学生年龄不能为0");

            RuleFor(a => a.LearnLevel)
                .NotEmpty().WithMessage("学生等级不能为空")
                .Must(checkLearnLevel).WithMessage("系统不存在该学生等级");

            RuleFor(a => a.TeacherId)
                .NotEqual(0).WithMessage("老师编号不能为0")
                .Must(checkTeacherId).WithMessage("系统不存在该老师编号");

        }
    }
}
